using AngleSharp.Dom;
using VladlenKazmiruk.Data;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        // Интерфейс для получения Data класса <T>
        public interface ICatcher<T>
        {
            public IEnumerable<T> Catch(); // Поиск и получение data object <T> по элементу
            
            public IElement? CurrentElement { get; } // Текущий dom элемент из Catch() (yield return)
            public IHtmlCollection<IElement> getElementCollection(); // Все элементы из topElement

            public void changeContext(IElement? context);
        }

        // Поиск и парсинг элементов по Context -> TopElement -> CurrentElement -> data object
        public abstract class BaseCather<T>  : ICatcher<T>
        {
            public event Action<T>? OnCatch;

            // Указываем context и topElement как аргументы для ясности, 
            // что они должны использоваться при поиске.
            // Использование по типу методов обратного вызова в getTopElement и GetElementCollection.

             // Поиск родителя с множеством элементов из контекста. Даем доступ к contextElement
            abstract protected IElement? locateTopElement(IElement contextElement); 
             // Поиск конкретных элементов для парсинга. даем доступ к topElement
            abstract protected IHtmlCollection<IElement>? locateElementCollection(IElement topElement);
             // Парсинг элемента в Data объект класса <T>
            abstract protected T initDataFromElement(IElement element);
            // Текущий dom элемент из Catch() (yield return)
            public IElement? CurrentElement { get => currentElement;} 

            // Расширяющие классы не имеют доступа к Context и TopElement
            IElement? contextElement; // Элемент контекста
            IElement? currentElement = null; // Присваиваем значение во время работы метода Catch()
            IElement? topElement = null; // Ленивая инициализация из метода getTopElement() либо Catch()
            IHtmlCollection<IElement>? elementCollection = null; // Аналогично предыдущему полю

            public BaseCather(IElement? contextElement) // Родительский элемент (контекст) при инициализации
            {
                //if (contextElement is null){
                //    throw new NullReferenceException("contextElement must not be null");
                //}

                this.contextElement = contextElement;
            }
            
            public IHtmlCollection<IElement> getElementCollection() // Проверка на NullReference и инициализация по требованию
            {
                return this.instancetNotNull(this.elementCollection, this.locateElementCollection, this.getTopElement());
            }

            public void changeContext(IElement? newContext)
            {
                if (newContext is null)
                {
                    throw new NullReferenceException("contextElement must not be null");
                }

                this.contextElement = newContext;
            }

            IElement getTopElement() // Проверка на NullReference и инициализация по требованию
            {
                return this.instancetNotNull(this.topElement, this.locateTopElement, this.contextElement);
            }

            // Абстракция парсинга элемента
            public IEnumerable<T> Catch()
            {
                var elements = this.getElementCollection();

                foreach (var el in elements)
                {
                    this.currentElement = el;

                    var data = this.initDataFromElement(el);

                    this.OnCatch?.Invoke(data);

                    yield return data;
                }
            }

            // Ленивая инициализация переменной с проверкой на null.
            //
            // Проверяем value на null
            // берем значения для инициализации из executeInitialization
            // передавая аргумент initArg, после его проверки на null.
            private N instancetNotNull<N>(N? value, Func<IElement, N?> executeInitialization, IElement? initArg)
            {
                if (value is null) // Если не инициализированна
                {
                    if (initArg == null)
                        throw new NullReferenceException("initArg for executeInitialization must not be null." +
                        "Use changeContext() to proper value.");

                    value = executeInitialization(initArg); // Обратный вызов из locateTopElement / locateElementCollection

                    if (value is null) // Если не нашли элемент
                        throw new NullReferenceException("Element was not found. NullReference return");

                    return value;
                }
                return value;
            }
        }

// Конкретные расширения класса BaseCather:

#region CarModelNameCatcher
        public class CarModelNameCatcher : BaseCather<string>
        {
            public CarModelNameCatcher(IElement? contextElement) : base(contextElement) 
            {
            }

            protected override string initDataFromElement(IElement element)
            {
                 var name = element.QuerySelector(Selectors.carModelName)?.TextContent;
                 if (name == null)
                    throw new NullReferenceException();
                 return name;
            }

            protected override IHtmlCollection<IElement>? locateElementCollection(IElement topElement)
            {
                return topElement.QuerySelectorAll(Selectors.carCell);
            }

            protected override IElement? locateTopElement(IElement contextElement)
            {
                var topLevelv1 = contextElement.QuerySelector(Selectors.carTopLevel);

                if (topLevelv1 is null)
                    return contextElement.QuerySelector(Selectors.carTopLevelV2);

                return contextElement.QuerySelector(Selectors.carTopLevel);
            }
        }
#endregion

#region CarModelInfoCatcher
        public class CarModelInfoCatcher : BaseCather<CarModel>
        {
            public CarModelInfoCatcher(IElement? contextElement) : base(contextElement)
            {
            }

            protected override CarModel initDataFromElement(IElement element)
            {
                    var carModel = new Data.CarModel(){
                        ComplectationCode = element.QuerySelector(Selectors.carModelComplCode)?.TextContent,
                        DateRange = element.QuerySelector(Selectors.carModelDates)?.TextContent,
                        Code = element.QuerySelector(Selectors.code)?.TextContent,
                        Url = element.QuerySelector("a")?.GetAttribute("href")
                    };

                    return carModel;
            }

            protected override IHtmlCollection<IElement>? locateElementCollection(IElement topElement)
            {
                return topElement.QuerySelectorAll(Selectors.carModel);
            }

            protected override IElement? locateTopElement(IElement contextElement)
            {
                return contextElement.QuerySelector(Selectors.carModelTop);
            }
        }
#endregion

#region ComplCatcher
        public class ComplCatcher : BaseCather<Data.Complectation>
        {
            List<string> dataTypes = new List<string>();

            public ComplCatcher(IElement? contextElement) : base(contextElement)
            {
            }

            protected override Complectation initDataFromElement(IElement element)
            {
                var compl = new Data.Complectation(){
                    ModCode = element.QuerySelector(Selectors.code)?.TextContent,
                    DateRange = element.QuerySelector(Selectors.dateRange)?.TextContent,
                    Url = element.QuerySelector("a")?.GetAttribute("href")
                };
                var datas = element.QuerySelectorAll(Selectors.complDataNames);
                var sels =  datas.Select(el => el.TextContent);

                var dataValues = element.QuerySelectorAll(Selectors.complDataValues)
                    .Select(el => el.TextContent).ToList();

                this.dataTypes = this.dataTypes.Distinct().ToList();

                compl.ModData = this.dataTypes.Zip(dataValues, 
                    (v1, v2) => new KeyValuePair<string,string>(v1, v2))
                    .ToList();
                dataValues.Clear();
                return compl;
                
            }

            protected override IHtmlCollection<IElement>? locateElementCollection(IElement topElement)
            {
                this.dataTypes.Clear();
                this.dataTypes = topElement.QuerySelectorAll("tr:nth-child(1)")[0].
                    QuerySelectorAll(Selectors.complDataNames).
                    Select(el =>  el.TextContent).ToList();

                return topElement.QuerySelectorAll(Selectors.complCell);
            }

            protected override IElement? locateTopElement(IElement contextElement)
            {
                return contextElement.QuerySelector(Selectors.complTop);
            }
        }
#endregion
    }
}