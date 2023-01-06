using AngleSharp;
using VladlenKazmiruk.Data;

namespace VladlenKazmiruk
{
    namespace Parser
    {
        public class IlCatsParser
        {
            public Car CarData { get => car.data; set => car.data = value; }
            public CarModel CarModelData { get => carModel.data; set => carModel.data = value; }
            public Complectation ComplectationData { get => Complectation.data; set => Complectation.data = value; }
            public Catalog_0 Catalog_0Data { get => catalog_0.data; set => catalog_0.data = value; }
            public Catalog_1 Catalog_1Data { get => catalog_1.data; set => catalog_1.data = value; }


            public IlCatsParser()
            {
                this.config = Configuration.Default.WithDefaultLoader();
                this.address = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
                this.context = BrowsingContext.New(this.config);
                this.domDocument = null;
            }

            public async void LoadPage()
            {
                this.domDocument = await context.OpenAsync(address);
            }



            domData<Car> car;
            domData<CarModel> carModel;
            domData<Complectation> Complectation;
            domData<Catalog_0> catalog_0;
            domData<Catalog_1> catalog_1;
            AngleSharp.Dom.IDocument? domDocument;
            IConfiguration config;
            string address;
            IBrowsingContext context;

            private struct domData<T>
            {
                public T data;
                public AngleSharp.Dom.IElement domElement;
            }
        }
    }
}