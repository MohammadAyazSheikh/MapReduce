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
            ls_Shopbuzz = crawl.GetData_Shopbuzz(Input);
            ls_Ishopping = crawl.GetData_Ishopping(Input);
            if (id == 1)
            {
                return Get_Min(ls_Shopbuzz); ;
            }
            else if (id == 2)
            {
                return Get_Min(ls_Ishopping);
            }
            else
            {
                return 0;
            }
            
        }
        public int Get_Min_Price(string Input)
        {
            ls_Shopbuzz = crawl.GetData_Shopbuzz(Input);
            ls_Ishopping = crawl.GetData_Ishopping(Input);

          
            int[] arr = new int[2];
            arr[0] = Get_Min(ls_Shopbuzz);
            arr[1] = Get_Min(ls_Ishopping);

            return arr.Min();
        }
    }
}
