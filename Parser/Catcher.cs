using AngleSharp.Dom;
using VladlenKazmiruk.Data;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        // Интерфейс для получения Data класса <T>
        interface ICatcher<T>
        {
            public IEnumerable<T> Catch(); // Поиск и получения Data класса <T> по элементу

            public IElement ContextElement { get; } // Родительский dom элемент для осуществления поиска
            public IElement? CurrentElement { get; } // Текущий dom элемент из Catch() (yield return)

            public IElement getTopElement(); // Top-level dom элемент для поиска <T>
            public IHtmlCollection<IElement> getElementCollection(); // Все элементы из topElement

            public void changeContext(IElement? context); 
        }

        public abstract class BaseCather<T> : ICatcher<T>
        {
            // Указываем context и topElement как аргументы для ясности, что они должны использоваться при поиске

             // Поиск родителя с элементами из контекста.
            abstract protected IElement? locateTopElement(IElement context); 
             // Поиск конкретных элементов для парсинга
            abstract protected IHtmlCollection<IElement>? locateElementCollection(IElement topElement);
             // Парсинг элемента в Data объект класса <T>
            abstract protected T initDataFromElement(IElement element);

            public IElement ContextElement { get => contextElement; }
            public IElement? CurrentElement { get => currentElement;} 

            IElement contextElement; //
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

            public IElement getTopElement() // Проверка на NullReference и инициализация по требованию
            {
                return this.instancetNotNull(this.topElement, this.locateTopElement, this.contextElement);
            }
            
            public IHtmlCollection<IElement> getElementCollection()
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

            // Ленивая инициализация переменной с проверкой на null
            private N instancetNotNull<N>(N? value, Func<IElement, N?> executeInitialization, IElement initArg)
            {
                if (value is null) // Если не инициализированна
                {
                    value = executeInitialization(initArg);

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

            protected override IHtmlCollection<IElement>? locateElementCollection()
            {
                return this.getTopElement().QuerySelectorAll(Selectors.carCellSelector);
            }

            protected override IElement? locateTopElement()
            {
                return base.ContextElement.QuerySelector(Selectors.carTopLevelSelector);
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

            protected override IHtmlCollection<IElement>? locateElementCollection()
            {
                return getTopElement().QuerySelectorAll(Selectors.carInfoSelector);
            }

            protected override IElement? locateTopElement()
            {
                return base.ContextElement.QuerySelector(Selectors.carInfoTopSelector);
            }
        }
#endregion
    }
}