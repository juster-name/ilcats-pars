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
            public IElement? CurrentElement { get; } // Nекущий dom элемент из Catch() (yield return)

            public IElement getTopElement(); // Top-level dom элемент для поиска <T>
        }

        public abstract class BaseCather<T> : ICatcher<T>
        {
            public IElement ContextElement { get => contextElement; }
            public IElement? CurrentElement { get => currentElement;} 

            IElement contextElement; // Используется только в конструкторе, контекст изменить нельзя

            protected IElement? currentElement = null; // Присваиваем значение во время работы метода Catch()
            IElement? topElement = null; // Ленивая инициализация из метода getTopElement() либо Catch()

            public BaseCather(IElement? contextElement) // Обяхательный родительский элемент (контекст) при инициализации
            {
                if (contextElement is null)
                {
                    throw new NullReferenceException();
                }

                this.contextElement = contextElement;
            }
            public IElement getTopElement() // Проверка на NullReference
            {
                if (this.topElement is null) // Если не инициализированна
                {
                    this.topElement = this.locateTopElement();

                    if (this.topElement is null) // Если не нашли элемент
                        throw new NullReferenceException("Top-Level Dom Element was not found");

                    return this.topElement;
                }
                return this.topElement;
            }
            abstract protected IElement? locateTopElement();
            abstract public IEnumerable<T> Catch();
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