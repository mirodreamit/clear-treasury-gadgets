using CT.Application.Abstractions.Interfaces;

namespace CT.Application.Abstractions.Models;

public class ContextualRequest : IContextualRequest
{
    private readonly Dictionary<string, object?> _context = new();

    public Dictionary<string, object?> Context
    { 
        get
        {
            return _context;
        }
    }
}
