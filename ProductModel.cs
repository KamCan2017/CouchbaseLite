namespace CBLiteApplication
{
    public class ProductModel: BaseModel
    {
        public ProductModel(): base()
        { }

        public string Name { get; set; }
        public double Price { get; set; }
    }
}
