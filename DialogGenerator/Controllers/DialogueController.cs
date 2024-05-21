using DialogGenerator.Dialog;
using DialogGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace DialogGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class DialogueController : ControllerBase
{
    private readonly DialogueGenerator _dialogueGenerator;

    public DialogueController(DialogueGenerator dialogueGenerator)
    {
        _dialogueGenerator = dialogueGenerator ?? throw new ArgumentNullException(nameof(dialogueGenerator));
    }

    [HttpPost]
    public IActionResult GetResponse([FromBody] DialogueRequest request)
    {
        if (request == null)
            return BadRequest("Request is null.");

        var response = _dialogueGenerator.GenerateResponse(request);
        return Ok(response);
    }
}