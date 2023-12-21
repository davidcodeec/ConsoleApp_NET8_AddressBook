﻿using ClassLibrary.Shared.Enums;
using ClassLibrary.Shared.Interfaces;

namespace ClassLibrary.Shared.Models.Responses;

public class ServiceResult<T> : IServiceResult<T>
{
    public T Result { get; set; }
    public ServiceStatus Status { get; set; }
}
