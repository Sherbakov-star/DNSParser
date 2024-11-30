using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace DNSParser
{
    public class Menu
    {
        public String Title { get; set; }
        public String Url { get; set; }
        public String UrlImage { get; set; }

        public List<Menu> Submenus { get; set; } = new List<Menu>();

    }

    public class Product
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public Double Price { get; set; }

    }

    internal class Program
    {
        static String baseUrl = "https://www.dns-shop.ru/";
        static String baseUrl2 = "https://www.dns-shop.ru/catalog/17a8d18c16404e77/duxovye-shkafy-elektricheskie/";
        static String baseUrl3 = "https://cks.group/catalog/burovoe_oborudovanie/avariynoe_oborudovanie/likvidiruyushchee/frezer/";
        static IWebDriver driver = new ChromeDriver();

        static void Main(string[] args)
        {
            //IWebDriver driver = new ChromeDriver();

            driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 20);
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 20);

            driver.Navigate().GoToUrl(baseUrl3);

            List<String> urlProducts = new List<string>();
            List<Product> resultProducts = new List<Product>();

            var root = driver.FindElement(By.ClassName("products-list__content"));

            var catalogs = root.FindElements(By.ClassName("catalog-products"));

            foreach (var catalog in catalogs)
            {
                var products = catalog.FindElements(By.ClassName("catalog-product"));
                foreach (var product in products)
                {
                    var nameRoot = product.FindElement(By.ClassName("catalog-product__name"));
                    String name = nameRoot.FindElement(By.TagName("span")).Text;

                    urlProducts.Add(
                        nameRoot.GetAttribute("href")
                        );
                }
            }

            urlProducts.ForEach(a =>
            {
                resultProducts.Add(ParseProduct(a));
            });

            var t = 0;
            /*var catalog = driver.FindElement(By.Id("catalog"));
            var root = catalog.FindElement(By.ClassName("catalog-menu-rootmenu"));
            var roots = root.FindElements(By.ClassName("catalog-menu__root-item"));

            List<Menu> rootMenu = new List<Menu>();

            foreach (var root_menu in roots)
            {
                var href = root_menu.FindElement(By.TagName("a"));

                Menu menu = new Menu
                {
                    Title = href.Text,
                    Url = href.GetAttribute("href"),
                };
                rootMenu.Add(menu);
            }
            test(rootMenu);*/



            var text = JsonSerializer.Serialize(resultProducts, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All) });

            try
            {
                File.WriteAllText("D:\\Projects\\DNSParser\\products.txt", text);
            }
            catch
            {
                Console.WriteLine("Hello, World!");
            } 

            Console.WriteLine("Hello, World!");
            Console.ReadKey();
        }

        public static Product ParseProduct(String url)
        {
            driver.Navigate().GoToUrl(url);
            var result = new Product();

            result.Name = driver.FindElement(By.ClassName("product-card-top__title")).Text;



            var price = driver.FindElement(By.ClassName("product-buy__price"));

            var t = price.Text.Split(' ');
            var priceStr = String.Join(String.Empty, t.Where(a => a != t.Last()));

            if (String.IsNullOrEmpty(priceStr))
            {
                priceStr = "15999";
            }

            result.Price = double.Parse(priceStr);

            var descRoot = driver.FindElement(By.ClassName("product-card-description-text"));
            var p = descRoot.FindElement(By.TagName("p"));

            result.Description = p.Text;

            return result;
        }

        public static void test(List<Menu> rootMenu)
        {
            foreach (var menu in rootMenu)
            {
                driver.Navigate().GoToUrl(menu.Url);

                IWebElement container = null;
                try
                {
                    container = driver.FindElement(By.ClassName("subcategory__item-container"));
                }
                catch
                {
                    continue;
                }

                var elements = container.FindElements(By.ClassName("subcategory__item"));

                foreach (var element in elements)
                {
                    var elementContent = element.FindElement(By.ClassName("subcategory__content"));
                    var title = elementContent.FindElement(By.ClassName("subcategory__title"));

                    var imageContainer = elementContent.FindElement(By.ClassName("subcategory__image"));
                    var picture = imageContainer.FindElement(By.TagName("picture"));
                    var image = picture.FindElement(By.TagName("img"));


                    Menu childMenu = new Menu { Title = title.Text, Url = element.GetAttribute("href"), UrlImage = image.GetAttribute("src") };
                    menu.Submenus.Add(childMenu);
                }
                test(menu.Submenus);

            }
        }

    }
}