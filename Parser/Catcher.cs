using AngleSharp.Dom;
using VladlenKazmiruk.Data;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        // Интерфейс для получения Data класса <T>
        interface ICatcher<T>
        {
            public IEnumerable<T> Catch(); // Поиск и получение data object <T> по элементу
            public IElement? CurrentElement { get; } // Текущий dom элемент из Catch() (yield return)
            public IHtmlCollection<IElement> getElementCollection(); // Все элементы из topElement

            public void changeContext(IElement? context);
        }

        // Поиск и парсинг элементов по Context -> TopElement -> CurrentElement -> data object
        public abstract class BaseCather<T> : ICatcher<T>
        {
            // Указываем context и topElement как аргументы для ясности, 
            // что они должны использоваться при поиске.
            // Использование по типу методов обратного вызова в getTopElement и GetElementCollection.

             // Поиск родителя с множеством элементов из контекста. Даем доступ к contextElement
            abstract protected IElement? locateTopElement(IElement context); 
             // Поиск конкретных элементов для парсинга. даем доступ к topElement
            abstract protected IHtmlCollection<IElement>? locateElementCollection(IElement topElement);
             // Парсинг элемента в Data объект класса <T>
            abstract protected T initDataFromElement(IElement element);
            // Текущий dom элемент из Catch() (yield return)
            public IElement? CurrentElement { get => currentElement;} 

            // Расширяющие классы не имеют доступа к Context и TopElement
            IElement contextElement; // Не может быть null, так как используется при поиске topElement
            IElement? currentElement = null; // Присваиваем значение во время работы метода Catch()
            IElement? topElement = null; // Ленивая инициализация из метода getTopElement() либо Catch()
            IHtmlCollection<IElement>? elementCollection = null; // Аналогично предыдущему полю

            public BaseCather(IElement? contextElement) // Обязательный родительский элемент (контекст) при инициализации
            {
                if (contextElement is null){
                    throw new NullReferenceException("contextElement must not be null");
                }

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

                    T data = this.initDataFromElement(el);
                    
                    yield return data;
                }
            }

            // Ленивая инициализация переменной с проверкой на null.
            //
            // Проверяем value на null
            // берем значения для инициализации из executeInitialization
            // передавая аргумент initArg.
            private N instancetNotNull<N>(N? value, Func<IElement, N?> executeInitialization, IElement initArg)
            {
                if (value is null) // Если не инициализированна
                {
                    value = executeInitialization(initArg); // Обратный вызов из locateTopElement / locateElementCollection

                    if (value is null) // Если не нашли элемент
                        throw new NullReferenceException("Element was not found. NullReference return");

                    return value;
                }
                return value;
            }
        }

#region CarsCatcher
        public class CarsCatcher : BaseCather<Data.Car>
        {
            public CarsCatcher(IElement? contextElement) : base(contextElement) 
            {
                
            }

            protected override Car initDataFromElement(IElement element)
            {
                var car = new Data.Car(){
                    Name =  element.QuerySelector(Selectors.carNameSelector)?.TextContent};

                return car;
            }

            protected override IHtmlCollection<IElement>? locateElementCollection(IElement topElement)
            {
                return topElement.QuerySelectorAll(Selectors.carCellSelector);
            }

            protected override IElement? locateTopElement(IElement context)
            {
                return context.QuerySelector(Selectors.carTopLevelSelector);
            }
        }
#endregion

#region CarModelCatcher
        public class CarModelCatcher : BaseCather<CarModel>
        {
            public CarModelCatcher(IElement? contextElement) : base(contextElement)
            {
            }

            protected override CarModel initDataFromElement(IElement element)
            {
                    var idEl = element.QuerySelector("a");

                    var carModel = new Data.CarModel(){
                        Url = idEl?.GetAttribute("href"),
                        Code = idEl?.TextContent,
                        DateRange = element.QuerySelector(Selectors.carDatesSelector)?.TextContent,
                        ComplectationCode = element.QuerySelector(Selectors.carComplCodeSelector)?.TextContent
                    };

                    return carModel;
            }

            protected override IHtmlCollection<IElement>? locateElementCollection(IElement topElement)
            {
                return topElement.QuerySelectorAll(Selectors.carInfoSelector);
            }

            protected override IElement? locateTopElement(IElement contextElement)
            {
                return contextElement.QuerySelector(Selectors.carInfoTopSelector);
            }
        }
#endregion
    }
}