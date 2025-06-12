using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using QuestPDF.Drawing;
using System.Globalization;

namespace InsuranceAPI.Services
{
    public class PolicyDocumentService : IPolicyDocumentService
    {
        public byte[] GeneratePolicyDocument(Insurance insurance)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    // COMPANY LETTERHEAD
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            try
                            {
                                row.RelativeItem().AlignLeft().Height(50).Image("logo.png");
                            }
                            catch
                            {
                                // If logo fails to load, just show text
                                row.RelativeItem().AlignLeft().Height(50).Text("INSUREWISE").Bold().FontSize(16);
                            } // Replace with your logo path
                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span("INSUREWISE INSURANCE CORPORATION").Bold().FontSize(14);
                                text.EmptyLine();
                                text.Span("123 Financial District, Suite 1000");
                                text.EmptyLine();
                                text.Span("Coimbatore, Cbe 10005");
                                text.EmptyLine();
                                text.Span("Phone: 5555 5555 | www.insurewise.com");
                            });
                        });

                        col.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Blue.Medium);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        // POLICY TITLE
                        col.Item().AlignCenter().Text(text =>
                        {
                            text.Span("INSURANCE POLICY").Bold().FontSize(16).FontColor(Colors.Blue.Darken3);
                            text.EmptyLine();
                            text.Span($"Policy Number: {insurance.InsurancePolicyNumber}").FontSize(12);
                        });

                        // DECLARATIONS PAGE
                        col.Item().Background(Colors.Grey.Lighten3).Padding(10).Text("DECLARATIONS").Bold().FontSize(12);

                        // POLICYHOLDER INFORMATION
                        col.Item().Text("POLICYHOLDER INFORMATION").Bold().FontSize(12).Underline();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Cell().Text("Legal Name:").SemiBold();
                            table.Cell().Text(insurance.Client.Name);

                            table.Cell().Text("Mailing Address:").SemiBold();
                            table.Cell().Text(insurance.Client.Address);

                            table.Cell().Text("Contact Email:").SemiBold();
                            table.Cell().Text(insurance.Client.Email);

            
                        });

                        // VEHICLE SCHEDULE
                        col.Item().Text("SCHEDULE OF COVERED AUTOMOBILES").Bold().FontSize(12).Underline();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Text("Year/Make/Model").SemiBold();
                                header.Cell().Text("VIN/Reg. No.").SemiBold();
                                header.Cell().Text("Type/Use").SemiBold();
                                header.Cell().Text("Seats").SemiBold();
                            });

                            // Data row
                            table.Cell().Text($"{insurance.Vehicle.RegistrationDate}/{insurance.Vehicle.MakerName}/{insurance.Vehicle.ModelName}");
                            table.Cell().Text(insurance.Vehicle.VehicleNumber);
                            table.Cell().Text($"{insurance.Vehicle.VehicleType} - Commercial");
                            table.Cell().Text(insurance.Vehicle.SeatCapacity.ToString());
                        });

                        // COVERAGE DETAILS
                        col.Item().Text("COVERAGE DETAILS").Bold().FontSize(12).Underline();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Text("Coverage Type").SemiBold();
                                header.Cell().Text("Limits").SemiBold();
                                header.Cell().Text("Deductible").SemiBold();
                            });

                            // Data rows
                            table.Cell().Text("Liability - Bodily Injury");
                            table.Cell().Text("$1,000,000 per accident");
                            table.Cell().Text("$1,000");

                            table.Cell().Text("Liability - Property Damage");
                            table.Cell().Text("$500,000 per accident");
                            table.Cell().Text("$1,000");

                            table.Cell().Text("Physical Damage - Comprehensive");
                            table.Cell().Text("Actual Cash Value");
                            table.Cell().Text("$1,000");

                            table.Cell().Text("Physical Damage - Collision");
                            table.Cell().Text("Actual Cash Value");
                            table.Cell().Text("$1,000");
                        });

                        // POLICY TERMS
                        col.Item().Text("POLICY TERMS").Bold().FontSize(12).Underline();
                        col.Item().Padding(10).Background(Colors.Grey.Lighten3).Column(terms =>
                        {
                            terms.Item().Text("1. Policy Period: " +
                                $"{insurance.InsuranceStartDate:MMMM d, yyyy} to " +
                                $"{insurance.InsuranceStartDate.AddYears(1):MMMM d, yyyy}");

                            terms.Item().Text("2. Premium Information:");
                            terms.Item().PaddingLeft(15).Text($"Annual Premium: {insurance.PremiumAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-IN"))}");
                            terms.Item().PaddingLeft(15).Text("Payment Options: 40% deposit, balance in installments");

                            terms.Item().Text("3. Cancellation: 30 days written notice required for cancellation");
                            terms.Item().Text("4. Territory: Coverage applies throughout the United States and Canada");
                            terms.Item().Text("5. Additional Provisions: See attached endorsements and forms");
                        });

                        // IMPORTANT NOTICES
                        col.Item().Text("IMPORTANT NOTICES").Bold().FontSize(12).Underline();
                        col.Item().Padding(5).Text(text =>
                        {
                            text.Span("This policy provides the coverage as specified above. Please read it carefully. " +
                                "The premium for this policy is based on the information you provided. " +
                                "If any of this information is incorrect or changes during the policy period, " +
                                "please notify us immediately as it may affect your coverage and premium.");
                            text.EmptyLine();
                            text.Span("Your policy contains exclusions, limitations, reductions of coverage, and terms under which the policy may be continued in force or discontinued.");
                        });

                        // SIGNATURE BLOCK
                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().Column(sigCol =>
                            {
                                sigCol.Item().Text("Authorized Representative").Underline();
                                sigCol.Item().PaddingTop(20).Text("_____________________________");
                                sigCol.Item().Text("Sarah Johnson, Underwriting Manager");
                                sigCol.Item().Text("Global Insurance Corporation");
                            });

                            row.RelativeItem().Column(dateCol =>
                            {
                                dateCol.Item().AlignRight().Text("Date of Issue").Underline();
                                dateCol.Item().AlignRight().PaddingTop(20).Text(DateTime.Today.ToString("MMMM d, yyyy"));
                            });
                        });
                    });

                    // FOOTER
                    page.Footer().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text("CONFIDENTIAL - For policyholder use only");
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}