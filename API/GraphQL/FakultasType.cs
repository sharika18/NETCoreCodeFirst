using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = DAL.Model;
namespace API.GraphQL
{
    public class FakultasType : ObjectType<Model.Fakultas>
    {
    }
}
