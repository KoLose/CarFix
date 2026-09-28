namespace Domain.Models;

public class Car
{
    public int Id { get; set; }
    public string Mark { get; set; }
    public string Model { get; set; }
    public string Year { get; set; }
    public string VIN { get; set; }
    public string Reg_number { get; set; }
    public int Mileage { get; set; }
    
    private Car(int id, string mark, string model, string year, string vin, string regNumber, int mileage)
    {
        Id = id;
        Mark = mark;
        Model = model;
        Year = year;
        VIN = vin;
        Reg_number = regNumber;
        Mileage = mileage;
    }
    
    public static Car Create(int id, string mark, string model, string year, string vin, string regNumber, int mileage)
    {
        return new Car(id, mark, model, year, vin, regNumber, mileage);
    }
}