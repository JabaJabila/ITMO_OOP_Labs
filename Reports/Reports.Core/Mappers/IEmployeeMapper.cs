using Core.Domain.Entities;
using Core.DTO;

namespace Core.Mappers
{
    public interface IEmployeeMapper : IMapper<EmployeeDto, Employee>
    {
    }
}