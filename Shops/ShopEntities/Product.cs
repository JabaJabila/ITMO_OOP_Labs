using Shops.Tools;

namespace Shops.ShopEntities
{
    public class Product
    {
        internal Product(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ProductException("Product must has it's own name");
        }

        public int Id { get; }
        public string Name { get; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var product = obj as Product;
            if (product == null)
                return false;
            return product.Id == Id && product.Name == Name;
        }
    }
}