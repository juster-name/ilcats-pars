using AngleSharp.Dom;
using VladlenKazmiruk.Data;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        // Интерфейс для получения Data класса <T>
        interface ICatcher<T>
        {
            public IElement ContextElement { get; } // Родительский dom элемент для осуществления поиска
            public IElement getElement(); // Top-level dom элемент для поиска <T>
            public T Catch(); // Поиск и получения Data класса <T> по элементу
        }

        public abstract class BaseCather<T> : ICatcher<T>
        {
            public IElement ContextElement { get => contextElement; }
            IElement contextElement;
            protected IElement? element = null;

            public BaseCather(IElement contextElement)
            {
                this.contextElement = contextElement;
            }

            abstract public IElement getElement();
            abstract public T Catch();
        }

    }
}