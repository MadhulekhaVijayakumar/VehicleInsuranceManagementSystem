select * from Clients
select * from Users
CREATE PROCEDURE proc_GenerateInsurancePolicyNumber
    @InsurancePolicyNumber VARCHAR(10) OUTPUT
AS
BEGIN
    DECLARE @NewId INT;

    SELECT @NewId = ISNULL(MAX(CAST(SUBSTRING(InsurancePolicyNumber, 3, LEN(InsurancePolicyNumber)) AS INT)), 500000) + 1
    FROM Insurances;

    SET @InsurancePolicyNumber = 'IP' + CAST(@NewId AS VARCHAR(8));
END

CREATE PROCEDURE GeneratePaymentId
AS
BEGIN
    DECLARE @NewId INT
    SELECT @NewId = ISNULL(MAX(PaymentId), 1000) + 1 FROM Payments
    SELECT @NewId AS PaymentId
END

