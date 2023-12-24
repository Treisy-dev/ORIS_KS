using System.Reflection;
using System.Text;
using MyTemplates;

namespace MyTemplates.Prog
{
    public class TemplateRenderer
    {
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        static void Main(string[] args)
        {
            TemplateEngine templateEngine = new TemplateEngine();

            string template = "Hello, @{Name}! You are @{Age} years old.\n";

            Person person = new Person
            {
                Name = "John",
                Age = 25
            };

            string result = templateEngine.RenderTemplate(template, person);

            Console.WriteLine(result);
        }
    }
}