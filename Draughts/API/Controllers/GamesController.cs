using Draughts.Core.Board;
using Draughts.Core.Board.Enum;
using Draughts.Core.Models;
using Draughts.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DraughtsGame.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameService gameService, ILogger<GamesController> logger)
    {
        Console.WriteLine("GamesController action called.");

        _logger = logger;
        _gameService = gameService;
        _logger.LogInformation("GamesController constructor called."); // Use proper logging
        Console.WriteLine("GamesController action called.");
    }

    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame([FromBody] CreateGameRequest request)
    {
        Console.WriteLine("CreateGame action called.");
        Console.WriteLine(request);
      /*  Console.WriteLine($"Received AiTimeLimit: {request?.AiTimeLimit}");
        Console.WriteLine($"Received AiTimeLimit: {request.AiTimeLimit}");
        Console.WriteLine($"Received HumanPlayerColor: {request.HumanPlayerColor}");
        Console.WriteLine($"Received HumanPlayerColor: {request?.HumanPlayerColor}");
*/
        Game game = await _gameService.CreateGame(request.HumanPlayerColor, request.AiTimeLimit);


        return Ok(game);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(Guid id)
    {
        try
        {
            var game = await _gameService.GetGame(id);
            return Ok(game);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("")]
    public BadRequestResult GetGame()
    {
        return BadRequest();
    }

    /*
    [HttpGet("/cc")]
    public BadRequestResult GetGame2()
    {

        return  BadRequest();

    }
    */
    [HttpPost("{id}/moves")]
    public async Task<ActionResult<Game>> MakeMove(Guid id, [FromBody] Move move)
    {
        try
        {
            var game = await _gameService.MakeMove(id, move);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/ai-move")]
    public async Task<ActionResult<Move>> GetAiMove(Guid id)
    {
        try
        {
            var move = await _gameService.GetAiMove(id);
            return Ok(move);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/hint")]
    public async Task<ActionResult<Move>> GetHint(Guid id)
    {
        try
        {
            var hint = await _gameService.GetHint(id);
            return Ok(hint);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class CreateGameRequest
{
    public PlayerColor HumanPlayerColor { get; set; }
    public int AiTimeLimit { get; set; } = 5000; // Default 5 seconds
}