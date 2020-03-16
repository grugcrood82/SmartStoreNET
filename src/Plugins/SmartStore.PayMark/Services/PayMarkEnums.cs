namespace SmartStore.PayMark.Services
{
	public enum PayMarkPaymentInstructionItem
	{
		Reference = 0,
		BankRoutingNumber,
		Bank,
		Bic,
		Iban,
		AccountHolder,
		AccountNumber,
		Amount,
		PaymentDueDate,
		Details
	}

	public enum PayMarkMessage
	{
		Message = 0,
		Event,
		EventId,
		State,
		Amount,
		PaymentId
	}

    public enum PayMarkPromotion
    {
        FinancingExample = 0,
        TextOnly
    }
}