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
        }

        public abstract class BaseCather<T> : ICatcher<T>
        {
            public IElement ContextElement { get => contextElement; }
            public IElement? CurrentElement { get => currentElement;} 

            IElement contextElement; // Используется только в конструкторе, контекст изменить нельзя

            protected IElement? currentElement = null; // Присваиваем значение во время работы метода Catch()
            IElement? topElement = null; // Ленивая инициализация из метода getTopElement() либо Catch()
            IHtmlCollection<IElement>? elementCollection = null; // Аналогично предыдущему полю

            public BaseCather(IElement? contextElement) // Обязательный родительский элемент (контекст) при инициализации
            {
                if (contextElement is null)
                {
                    throw new NullReferenceException("contextElement must not be null");
                }

                this.contextElement = contextElement;
            }

            public IElement getTopElement() // Проверка на NullReference и инициализация по требованию
            {
                return this.initIfNotNull(this.topElement, this.locateTopElement);
            }
            
            public IHtmlCollection<IElement> getElementCollection()
            {
                return this.initIfNotNull(this.elementCollection, this.locatElementCollection);
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

            abstract protected IElement? locateTopElement(); // Поиск родителя с элементами
            abstract protected IHtmlCollection<IElement>? locatElementCollection(); // Поиск конкретных элементов для парсинга
            abstract protected T initDataFromElement(IElement element); // Парсинг элемента в Data объект класса <T>
            
            // Ленивая инициализация переменной с проверкой на null
            private N initIfNotNull<N>(N? value, Func<N?> executeInitialization)
            {
                if (value is null) // Если не инициализированна
                {
                    value = executeInitialization();

                    if (value is null) // Если не нашли элемент
                        throw new NullReferenceException("Top-Level Dom Element was not found");

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

            public override IEnumerable<Data.Car> Catch()
            {
                var carCells = this.getTopElement().QuerySelectorAll(Selectors.carCellSelector);

                foreach (var cellEl in carCells)
                {
                    var carNameEl = cellEl.QuerySelector(Selectors.carNameSelector);
                    base.currentElement = carNameEl;

                    var car = new Data.Car(){
                        Name = carNameEl?.TextContent};
                    
                    yield return car;
                }
            }

            protected override IElement? locateTopElement()
            {
                return base.ContextElement.QuerySelector(Selectors.carTopLevelSelector);
            }
        }
#endregion

    }
}