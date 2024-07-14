using ConsoleResult;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        HttpClientHandler handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

        using (HttpClient client = new HttpClient(handler))
        {
            client.BaseAddress = new Uri("https://localhost:7021/");

            IOrderDetailsService orderService = new OrderDetailsService(client);

            bool running = true;
            while (running)
            {
                Console.WriteLine("Выберите пункт меню:");
                Console.WriteLine("1 - Вставить в базу данных JSON");
                Console.WriteLine("2 - Поиск значений из сохранённого в базе данных JSON по ID");
                Console.WriteLine("3 - Модифицировать значение произвольного поля в сохранённом JSON");
                Console.WriteLine("4 - Удалить из базы сохраненные JSON");
                Console.WriteLine("exit - Выход");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Введите дату (гггг-мм-дд):");
                            DateOnly date = DateOnly.Parse(Console.ReadLine());
                            Console.WriteLine("Введите имя:");
                            string name = Console.ReadLine();
                            Console.WriteLine("Введите количество:");
                            int count = int.Parse(Console.ReadLine());
                            Console.WriteLine("Введите стоимость:");
                            int cost = int.Parse(Console.ReadLine());
                            await orderService.InsertJsonToDatabaseAsync(date, name, count, cost);
                            break;
                        case "2":
                            Console.WriteLine("Введите ID для поиска:");
                            int searchId = int.Parse(Console.ReadLine());
                            await orderService.GetOrderDetailByIdAsync(searchId);
                            break;
                        case "3":
                            Console.WriteLine("Введите ID записи для модификации:");
                            int id = int.Parse(Console.ReadLine());
                            Console.WriteLine("Введите новую дату (гггг-мм-дд) или оставьте пустым для пропуска:");
                            string dateInput = Console.ReadLine();
                            DateOnly? newDate = string.IsNullOrEmpty(dateInput) ? (DateOnly?)null : DateOnly.Parse(dateInput);

                            Console.WriteLine("Введите новое имя или оставьте пустым для пропуска:");
                            string newName = Console.ReadLine();
                            newName = string.IsNullOrEmpty(newName) ? null : newName;

                            Console.WriteLine("Введите новое количество или оставьте пустым для пропуска:");
                            string countInput = Console.ReadLine();
                            int? newCount = string.IsNullOrEmpty(countInput) ? (int?)null : int.Parse(countInput);

                            Console.WriteLine("Введите новую стоимость или оставьте пустым для пропуска:");
                            string costInput = Console.ReadLine();
                            int? newCost = string.IsNullOrEmpty(costInput) ? (int?)null : int.Parse(costInput);

                            await orderService.ModifyJsonFieldInDatabaseAsync(id, newDate, newName, newCount, newCost);
                            break;
                        case "4":
                            Console.WriteLine("Введите ID записи для удаления:");
                            int deleteId = int.Parse(Console.ReadLine());
                            await orderService.DeleteJsonFromDatabaseAsync(deleteId);
                            break;
                        case "exit":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Неверный выбор, попробуйте снова.");
                            break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine($"Неверный формат ввода: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Произошла ошибка: {e.Message}");
                }
            }
        }
    }
}