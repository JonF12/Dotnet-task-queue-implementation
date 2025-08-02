using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetQueue.Services;

public interface IGenericService 
{ 
    Task<bool> SomeDependencyMethod();
}
public class GenericService : IGenericService
{
    public async Task<bool> SomeDependencyMethod() 
    { 
        await Task.Delay(2000);
        return true;
    }
}
