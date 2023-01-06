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
            public IElement? CurrentElement { get => currentElement; } 

            IElement contextElement; // Используется только в конструкторе, контекст изменить нельзя

            protected IElement? currentElement = null; // Присваиваем значение во время работы метода Catch()
            protected IElement? topElement = null; // Ленивая инициализация из метода getTopElement() либо Catch()

            public BaseCather(IElement? contextElement) // Обяхательный родительский элемент (контекст) при инициализации
            {
                if (contextElement is null)
                {
                    throw new NullReferenceException();
                }

                this.contextElement = contextElement;
            }
            abstract public IElement getTopElement();
            abstract public IEnumerable<T> Catch();
        }

        public class CarsCatcher : BaseCather<Data.Car>
        {

            public CarsCatcher(IElement? contextElement) : base(contextElement) {}

            public override IEnumerable<Data.Car> Catch()
            {
                var carCells = this.getTopElement().QuerySelectorAll(Selectors.carCellSelector);

                foreach (var cellEl in carCells)
                {
                    var car = new Data.Car();
                    var carNameEl = cellEl.QuerySelector(Selectors.carNameSelector);
                    car.Name = carNameEl?.TextContent;
                    base.currentElement = carNameEl;
                    yield return car;
                }
            }

            public override IElement getTopElement()
            {
                if (base.topElement == null)
                {
                    // Possible Null Reference от QuerySelector не актуален - элемент всегда присутствует.
                    base.topElement = base.ContextElement.QuerySelectorAll(Selectors.carTopLevelSelector)[0];
                    return base.topElement;
                }
                return base.topElement;
            }
        }

    }
}