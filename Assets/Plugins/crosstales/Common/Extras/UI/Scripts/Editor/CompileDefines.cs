#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.UI
{
   /// <summary>Adds the given define symbols to PlayerSettings define symbols.</summary>
   [InitializeOnLoad]
   public class CompileDefines : Crosstales.Common.EditorTask.BaseCompileDefines
   {
      private const string SYMBOL = "CT_UI";

      static CompileDefines()
      {
         addSymbolsToAllTargets(SYMBOL);
      }
   }
}
#endif
// © 2020-2023 crosstales LLC (https://www.crosstales.com)