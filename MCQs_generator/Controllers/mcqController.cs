using MCQs_generator.data;
using MCQs_generator.model;
using MCQs_generator.service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MCQController : ControllerBase
{
    private readonly GeminiService _gemini;
    private readonly AppDbContext _db;

    public MCQController(GeminiService gemini, AppDbContext db)
    {
        _gemini = gemini;
        _db = db;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateAndSave([FromBody] mcqRequest request)
    {
        var mcqs = await _gemini.GetMCQsFromGeminiAsync(request.topic);

        foreach (var mcq in mcqs)
        {
            mcq.topic = request.topic;
        }

        await _db.MCQs.AddRangeAsync(mcqs);
        await _db.SaveChangesAsync();

        return Ok(mcqs);
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        return Ok(_db.MCQs.ToList());
    }

    [HttpGet("by-topic/{topic}")]
    public IActionResult GetByTopic(string topic)
    {
        var result = _db.MCQs.Where(m => m.topic == topic).ToList();
        return Ok(result);
    }
}
