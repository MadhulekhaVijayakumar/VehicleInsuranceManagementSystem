using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InsuranceAPI.Services
{
    public class QuotePdfGenerator:IQuotePdfGenerator
    {
        public byte[] GenerateQuotePdf(
            string quoteNumber,
            Client client,
            Vehicle vehicle,
            Proposal proposal,
            InsuranceDetails insurance,
            List<string> addOns = null)
        {
            return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("InsureWise").FontSize(20).Bold().AlignCenter();
                    
                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Quote No: {quoteNumber}");
                            row.RelativeItem().Text($"Date: {DateTime.Now:dd MMM yyyy}");
                            row.RelativeItem().Text($"Valid Till: {DateTime.Now.AddDays(15):dd MMM yyyy}");
                        });

                        column.Item().Element(e => e
    .PaddingVertical(5)
    .BorderBottom(1)
    .BorderColor(Colors.Grey.Lighten2));


                        column.Item().Text("Client Details").FontSize(14).Bold();
                        column.Item().Text($"Name: {client.Name}");
                        column.Item().Text($"Email: {client.Email}");
                        column.Item().Text($"Address: {client.Address}");
                        column.Item().Text($"Aadhaar: {client.AadhaarNumber}, PAN: {client.PANNumber}");

                        column.Item().Element(e => e.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2));

                        column.Item().Text("Vehicle Details").FontSize(14).Bold();
                        column.Item().Text($"Number: {vehicle.VehicleNumber}");
                        column.Item().Text($"Type: {vehicle.VehicleType}, Fuel: {vehicle.FuelType}");
                        column.Item().Text($"Make & Model: {vehicle.MakerName} {vehicle.ModelName}");
                        column.Item().Text($"Engine No: {vehicle.EngineNumber}, Chassis No: {vehicle.ChassisNumber}");
                        column.Item().Text($"Year: {vehicle.RegistrationDate}");

                        column.Item().Element(e => e.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2));

                        column.Item().Text("Policy Details").FontSize(14).Bold();
                        column.Item().Text($"Insurance Type: {proposal.InsuranceType}");
                        column.Item().Text($"Period: {insurance.InsuranceStartDate:dd MMM yyyy} to {insurance.InsuranceStartDate.AddYears(1):dd MMM yyyy}");
                        if (addOns != null && addOns.Any())
                            column.Item().Text("Add-ons: " + string.Join(", ", addOns));
                        else
                            column.Item().Text("Add-ons: None");

                        column.Item().Element(e => e.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2));

                        column.Item().Text("Premium Summary").FontSize(14).Bold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            void Row(string label, decimal amount)
                            {
                                table.Cell().Element(CellStyle).Text(label);
                                table.Cell().Element(CellStyle).AlignRight().Text($"₹{amount:N2}");
                            }


                            table.Cell().Element(CellStyle).Text("Item").Bold();
                            table.Cell().Element(CellStyle).Text("Amount (INR)").Bold();

                            Row("Insured Declared Value (IDV)", insurance.InsuranceSum);
                            Row("Base Premium", insurance.CalculatedPremium * 0.85M);
                            Row("Add-on Charges", insurance.CalculatedPremium * 0.10M);
                            Row("GST (5%)", insurance.CalculatedPremium * 0.05M);
                            Row("Total Premium", insurance.CalculatedPremium);

                            static IContainer CellStyle(IContainer container) =>
                                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
                        });

                        column.Item().PaddingTop(10).Text("Terms and Conditions").FontSize(14).Bold();
                        column.Item().Text("• This is a system-generated quote and does not guarantee coverage.");
                        column.Item().Text("• Final premium may vary based on vehicle inspection and risk assessment.");
                        column.Item().Text("• Quote is valid for 15 days from the date of issue.");
                    });

                    page.Footer().AlignCenter().Text("Generated by Vehicle Insurance System").FontSize(10).Italic();
                });
            }).GeneratePdf();
        }
    }

}
