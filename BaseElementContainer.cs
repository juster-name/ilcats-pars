using AngleSharp.Dom;

namespace VladlenKazmiruk
{
    /*
        Базовые классы с информацией о машине будут инкапсулировать ссылку на DOM элемент
        для будущего редактирования, поиска и добавления данных в классы.
    */
    public abstract class DomElementContainer
    {
        public IElement DomElement { get => domElement; set => domElement = value; }
        IElement domElement;

        public DomElementContainer(IElement domElement)
        {
            this.domElement = domElement;
        }
    }
}