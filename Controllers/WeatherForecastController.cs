using Microsoft.AspNetCore.Mvc;
using TheTicketShop.DTOs;

namespace TheTicketShop.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost("BinarySearch")]
    public string BinarySearch([FromBody] BinarySearchRequest request)
    {
        var sortedList = QuickSort(request.Collection);
        int first = 0;
        int last = sortedList.Count - 1;

        while (first <= last)
        {
            int midPoint = (first + last) / 2;
            if (sortedList[midPoint] == request.Target)
            {
                return $"there is result: your index: {midPoint}, and your sorted list: {string.Join(", ", sortedList)}";
            }
            else if (string.Compare(sortedList[midPoint], request.Target) < 0)
            {
                first = midPoint + 1;
            }
            else 
            {
                last = midPoint - 1;
            }
        }
        return "none";
    }

    [HttpPost("SelectSort")]
    public List<int> SelectSort([FromBody] List<int> unsortedList)
    {
        List<int> result = new List<int>();
        while (unsortedList.Count > 0)
        {
            int firstIndex = 0;
            int lowestInt = unsortedList[0];
            for (var i = 1; i < unsortedList.Count; i++)
            {
                if (unsortedList[i] < lowestInt)
                {
                    lowestInt = unsortedList[i];
                    firstIndex = i;
                }
            }
            result.Add(lowestInt);
            unsortedList.RemoveAt(firstIndex);
        }
        return result;
    }

    [HttpPost("QuickSort")]
    public List<string> QuickSort([FromBody] List<string> unsortedList)
    {
        if (unsortedList.Count <= 1)
            return unsortedList;
        
        List<string> lessThanPivot = new List<string>();
        List<string> greaterThanPivot = new List<string>();
        string pivot = unsortedList[0];

        for (var i = 1; i < unsortedList.Count; i++)
        {
            if (string.Compare(unsortedList[i], pivot) < 0)
                lessThanPivot.Add(unsortedList[i]);
            if (string.Compare(unsortedList[i], pivot) > 0)
                greaterThanPivot.Add(unsortedList[i]);
        }
        List<string> result = [.. QuickSort(lessThanPivot), pivot, .. QuickSort(greaterThanPivot)];
        
        return result;
    }

    [HttpPost("MergeSort")]
    public List<int> MergeSort([FromBody] List<int> unsortedList)
    {
        if (unsortedList.Count <= 1)
            return unsortedList;

        int middleIndex = unsortedList.Count / 2;
        List<int> leftValues = MergeSort(unsortedList.GetRange(0, middleIndex));
        List<int> rightValues = MergeSort(unsortedList.GetRange(middleIndex, unsortedList.Count - middleIndex));
        int leftIndex = 0;
        int rightIndex = 0;
        List<int> result = new List<int>();

        while (leftIndex < leftValues.Count && rightIndex < rightValues.Count)
        {
            if (leftValues[leftIndex] <= rightValues[rightIndex])
            {
                result.Add(leftValues[leftIndex]);
                leftIndex++;
            }
            else
            {
                result.Add(rightValues[rightIndex]);
                rightIndex++;
            }
        }

        while (leftIndex < leftValues.Count)
        {
            result.Add(leftValues[leftIndex]);
            leftIndex++;
        }

        while (rightIndex < rightValues.Count)
        {
            result.Add(rightValues[rightIndex]);
            rightIndex++;
        }

        return result;
    }
}
