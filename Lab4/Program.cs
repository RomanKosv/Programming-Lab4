
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;

int[]? array = null;

Random rand = new Random(DateTime.Now.Nanosecond);

void SetArray(out int[] array, int lenght, IEnumerable<int> enumer)
{
    array = new int[lenght];
    IEnumerator<int> iter = enumer.GetEnumerator();
    for (int i = 0; i < lenght; i++)
    {
        iter.MoveNext();
        array[i] = iter.Current;
    }
}

void InputLenght(out int lenght)
{
    Console.Write("Введите длину массива:");
    while (!int.TryParse(Console.ReadLine(), out lenght) || lenght < 0 || lenght > Array.MaxLength)
        Console.Write($"Длина должна быть целым неотриательным числом не более {Array.MaxLength}.\nВведите длину массива:");
}

void InputCount(out int lenght)
{
    Console.Write("Введите количество элементов:");
    while (!int.TryParse(Console.ReadLine(), out lenght) || lenght < 0)
        Console.Write("Количество элементов должно быть целым неотрицательным числом.\nВведите количество элементов:");
}

void InputElement(out int element)
{
    while (!int.TryParse(Console.ReadLine(), out element)) Console.Write("Элемент должен быть целым числом.\nВведите элемент:");
}

void DisplayArray(int[] array, bool requireFull=false)
{
    for(int i = 0; i < array.Length - 1; i++)
    {
        Console.Write($"{array[i]} ");
        if (!requireFull && i==100) {
            Console.Write("... ");
            break;
        }
    }
    Console.WriteLine($"{array[^1]}");
}

void CreateMessage()
{
    if (array.Length == 0) Console.WriteLine("Создан пустой массив");
    else
    {
        Console.WriteLine("Создан массив:");
        DisplayArray(array);
    }
}

void ChangeMessage()
{
    if (array.Length == 0) Console.WriteLine("Измененный массив пуст");
    else
    {
        Console.WriteLine("Измененный массив:");
        DisplayArray(array);
    }
}

void NullMesssage()
{
    Console.WriteLine("Для выполнения данной команды массив должен быть инициализирован.");

}

IEnumerable<int> Randoms()
{
    for (; ; ) yield return rand.Next(-100, 100);
}

IEnumerable<int> Inputs()
{
    for (int i = 1; ; i++)
    {
        Console.WriteLine($"Введите элемент {i}:");
        InputElement(out int element);
        Console.WriteLine(element);
        yield return element;
    }
}

bool IsSorted(int[] array)
{
    for (int i = 1; i < array.Length; i++)
        if (array[i - 1] > array[i]) return false;
    return true;
}

bool CheckAddCount(int count)
{
    if (Array.MaxLength - array.Length < count)
    {
        Console.WriteLine("Массив не может быть такой длины.");
        return false;
    }
    return true;
}

const String helpMessage = """
Для работы с массивом введите одну из команд:
create для создания или пересоздания массива
print для вывода массива на экран
removing для удаления четных элементов
add для добавления нескольких элементов в конец массива
move для перемещения положительных элементов в начало, а отрицательных - в конец
first для поиска первого отрицательного числа
sort для сортировки
find для поиска конкретного значения
""";

bool fStop = false;

while (!fStop)
{
    Console.WriteLine("Введите команду или help для вывода инструкции:");
    switch (Console.ReadLine())
    {
        case "help":
            Console.WriteLine(helpMessage);
            break;
        case "create":
            Console.WriteLine("Введите режим создания или help");
            switch (Console.ReadLine())
            {
                case "help":
                    Console.WriteLine("""
                    Введите одну из следующих команд, для выбора режима создания массива:
                    random для создания из случайных чисел от -100 до 100
                    input для вводя с клавиатуры
                    """);
                    break;
                case "random":
                    InputLenght(out int lenght);
                    SetArray(out array, lenght, Randoms());
                    CreateMessage();
                    break;
                case "input":
                    InputLenght(out lenght);
                    SetArray(out array, lenght, Inputs());
                    CreateMessage();
                    break;
                default:
                    Console.WriteLine("Такого режима нет");
                    break;
            }
            break;
        case "move":
            if (array != null)
            {
                SetArray(out array, array.Length, array.Where(i => i > 0).Concat(array.Where(i => i == 0)).Concat(array.Where(i => i < 0)));
                ChangeMessage();
            }
            else NullMesssage();
            break;
        case "removing":
            if (array != null)
            {
                SetArray(out array, array.Count(i => i % 2 != 0), array.Where(i => i % 2 != 0));
                ChangeMessage();
            }
            else NullMesssage();
            break;
        case "add":
            if (array != null)
            {
                Console.WriteLine("Введите режим добавления элементов или help:");
                switch (Console.ReadLine())
                {
                    case "help":
                        Console.WriteLine("""
                        Введите одну из следующих команд, для выбора режима добавления элементов:
                        random для создания из случайных чисел от -100 до 100
                        input для вводя с клавиатуры
                        """);
                        break;
                    case "random":
                        InputCount(out int count);
                        if (CheckAddCount(count))
                        {
                            SetArray(out array, array.Length + count, array.Concat(Randoms()));
                            ChangeMessage();
                        }
                        break;
                    case "input":
                        InputCount(out count);
                        if (CheckAddCount(count))
                        {
                            SetArray(out array, array.Length + count, array.Concat(Inputs()));
                            ChangeMessage();
                        }
                        break;
                    default:
                        Console.WriteLine("Такго режима нет");
                        break;
                }
            }
            else NullMesssage();
            break;
        case "first":
            if (array != null)
            {
                bool f = false;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] < 0)
                    {
                        f = true;
                        Console.WriteLine($"Первый отрицательный элемент на позиции {i + 1}, его значение {array[i]}");
                        break;
                    }
                }
                if (!f) Console.WriteLine("В массиве нет отрицательных элементов");
            }
            else NullMesssage();
            break;
        case "find":
            if (array != null)
            {
                if (IsSorted(array))
                {
                    int element;
                    Console.Write("Введите искомый элемент:");
                    while (!int.TryParse(Console.ReadLine(), out element)) Console.Write("Искомый элемент должен быть целым числом.\nВведите искомый элемент:");
                    int start = 0;
                    int end = array.Length;
                    int compares = 0;
                    while (end - start > 1 && array[start] != element)
                    {
                        compares+=2;
                        if (array[(start + end) / 2] <= element) start = (start + end) / 2;
                        else end = (start + end) / 2;
                        compares++;
                    }
                    compares++;
                    if (end - start > 1) compares++;
                    if (array.Length != 0 && array[start] == element) Console.WriteLine($"Искомое число находится на позиции {start + 1}, потребовалось {compares+2} сравнений.");
                    else Console.WriteLine($"В массиве нет искомого числа, потребовалось {compares} сравнений.");
                }
                else Console.WriteLine("Массив должен быть отсортирован");
            }
            else NullMesssage();
            break;
        case "sort":
            if (array != null)
            {
                Console.WriteLine("Введите способ сортировки или help:");
                switch (Console.ReadLine())
                {
                    case "choose":
                        for (int start = 0; start < array.Length - 1; start++)
                        {
                            int mini = start;
                            for (int i = start; i < array.Length; i++)
                                if (array[i] < array[mini]) mini = i;
                            (array[mini], array[start]) = (array[start], array[mini]);
                        }
                        ChangeMessage();
                        break;
                    case "help":
                        Console.WriteLine("""
                        Введите одну из следующих команд, для выбора режима сортировки:
                        choose для сортировки выбором
                        """);
                        break;
                    default:
                        Console.WriteLine("Такого способа нет");
                        break;
                }
            }
            else NullMesssage();
            break;
        case "stop":
            fStop = true;
            break;
        case "print":
            if (array == null) NullMesssage();
            else if (array.Length == 0) Console.WriteLine("Текущий массив пуст");
            else
            {
                Console.WriteLine("Текцщий массив:");
                DisplayArray(array, true);
            }
            break;
        case "":
            break;
        default:
            Console.WriteLine("Такой команды не существует");
            break;
    }
}