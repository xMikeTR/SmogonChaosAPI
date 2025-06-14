//Simple input validation interface
public interface IInputValidators
{
    bool IsValidDate(string date);
    bool IsValidFormat(string format);
    bool IsValidPokemon(string selected);
}