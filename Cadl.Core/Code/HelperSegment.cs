using System;
namespace Cadl.Core.Code
{
    public class HelperSegment : Segment
    {
        private const string asyncIterate = @"
async function iterate(array, callback) {
    for (let index = 0; index < array.length; index++) {
      await callback(array[index], index, array);
    }
  }";

        public HelperSegment(int indentCount)
            : base(indentCount)
        {
            Methods.Add(asyncIterate);
        }
    }
}
