using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Product
    {
        static List<Product> ls_Shopbuzz = new List<Product>();
        static List<Product> ls_Ishopping = new List<Product>();
        static List<Product> ls_HomeShopping = new List<Product>();
        static List<Product> ls_PriceOye = new List<Product>();
        Crawler crawl = new Crawler();
        public string name { get; set; }
        public int price { get; set; }

        public int Get_Min(List<Product> pro)
        {
            int len = pro.Count;
            int[] arr = new int[len];
            int index = 0;
            foreach (var item in pro)
            {
                arr[index] = item.price;
                index++;
            }

            return arr.Min();
        }

        public int FindMin(string Input,int id)
        {
            
            if (id == 1)
            {
                ls_PriceOye = crawl.GetData_PriceOye(Input);
                return Get_Min(ls_PriceOye);
            }
            else if (id == 2)
            {
                ls_Ishopping = crawl.GetData_Ishopping(Input);
                return Get_Min(ls_Ishopping);
            }
            else if (id == 3)
            {
                ls_HomeShopping = crawl.GetData_HomeShopping(Input);
                return Get_Min(ls_HomeShopping);
            }
            else if (id == 4)
            {
                ls_Shopbuzz = crawl.GetData_Shopbuzz(Input);
                return Get_Min(ls_Shopbuzz);
            }
            else
            {
                return 0;
            }
            
        }
    }
}
