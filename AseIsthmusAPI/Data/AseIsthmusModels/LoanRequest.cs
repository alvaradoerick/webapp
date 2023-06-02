﻿using System;
using System.Collections.Generic;
using AseIsthmusAPI.Data.AseIsthmusModels;

namespace AseIsthmusAPI.Data;

public partial class LoanRequest
{
    public int LoanRequestId { get; set; }

    public int LoansTypeId { get; set; }

    public decimal AmountRequested { get; set; }

    public int Term { get; set; }

    public DateTime RequestedDate { get; set; }

    public string PersonId { get; set; } = null!;

    public string BankAccount { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsApproved { get; set; }

    public DateTime ApprovedDate { get; set; }

    public virtual LoansType LoansType { get; set; } = null!;

    public virtual User Person { get; set; } = null!;
}