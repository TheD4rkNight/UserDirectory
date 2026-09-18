namespace UserDirectory.Domain.Entities;

public sealed class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Age { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Pincode { get; private set; } = string.Empty;

    private User() { }

    public User(string name, int age, string city, string state, string pincode)
    {
        Update(name, age, city, state, pincode);
    }

    public void Update(string name, int age, string city, string state, string pincode)
    {
        Name = name.Trim();
        Age = age;
        City = city.Trim();
        State = state.Trim();
        Pincode = pincode.Trim();
    }
}
