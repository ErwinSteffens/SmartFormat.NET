using System.Reflection;
using SmartFormat.Core.Extensions;
using SmartFormat.Utilities;
using System.Linq;

namespace SmartFormat.Extensions
{
	public class ReflectionSource : ISource
	{
		public ReflectionSource(SmartFormatter formatter)
		{
			// Add some special info to the parser:
			formatter.Parser.AddAlphanumericSelectors(); // (A-Z + a-z)
			formatter.Parser.AddAdditionalSelectorChars("_");
			formatter.Parser.AddOperators(".");
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			var current = selectorInfo.CurrentValue;
			var selector = selectorInfo.SelectorText;

			if (current == null)
			{
				return false;
			}

			// REFLECTION:
			// Let's see if the argSelector is a Selectors/Field/ParseFormat:
			var sourceType = current.GetType();

#if NET35 || NET40
            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
			bindingFlags |= selectorInfo.FormatDetails.Settings.GetCaseSensitivityBindingFlag();

			var members = sourceType.GetMember(selector, bindingFlags);
			foreach (var member in members)
			{
				switch (member.MemberType)
				{
					case MemberTypes.Field:
						//  Selector is a Field; retrieve the value:
						var field = (FieldInfo) member;
						selectorInfo.Result = field.GetValue(current);
						return true;
					case MemberTypes.Property:
					case MemberTypes.Method:
						MethodInfo method;
						if (member.MemberType == MemberTypes.Property)
						{
							//  Selector is a Property
							var prop = (PropertyInfo) member;
							//  Make sure the property is not WriteOnly:
							if (prop.CanRead)
							{
								method = prop.GetGetMethod();
							}
							else
							{
								continue;
							}
						}
						else
						{
							//  Selector is a method
							method = (MethodInfo) member;
						}

						//  Check that this method is valid -- it needs to return a value and has to be parameterless:
						//  We are only looking for a parameterless Function/Property:
						if (method.GetParameters().Length > 0)
						{
							continue;
						}

						//  Make sure that this method is not void!  It has to be a Function!
						if (method.ReturnType == typeof(void))
						{
							continue;
						}

						//  Retrieve the Selectors/ParseFormat value:
						selectorInfo.Result = method.Invoke(current, new object[0]);
						return true;

				}
			}
#else
            var typeInfo = sourceType.GetTypeInfo();

            var caseSensitive = selectorInfo.FormatDetails.Settings.GetCaseSensitivityComparison();
            var members = typeInfo.GetAllMembers().Where(m => m.Name.Equals(selector, caseSensitive));
            foreach (var member in members)
            {
                var fieldInfo = member as FieldInfo;
                if (fieldInfo != null)
                {
                    selectorInfo.Result = fieldInfo.GetValue(current);
                    return true;
                }

                if (member is PropertyInfo || member is MethodInfo)
                {
                    MethodInfo methodInfo;

                    var propertyInfo = member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        //  Make sure the property is not WriteOnly:
                        if (propertyInfo.CanRead)
                        {
                            methodInfo = propertyInfo.GetMethod;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // Selector is a method
                        methodInfo = (MethodInfo)member;
                    }

                    //  Check that this method is valid -- it needs to return a value and has to be parameterless:
                    //  We are only looking for a parameterless Function/Property:
                    if (methodInfo.GetParameters().Length > 0)
                    {
                        continue;
                    }

                    //  Make sure that this method is not void!  It has to be a Function!
                    if (methodInfo.ReturnType == typeof(void))
                    {
                        continue;
                    }

                    //  Retrieve the Selectors/ParseFormat value:
                    selectorInfo.Result = methodInfo.Invoke(current, new object[0]);
                    return true;
                }
            }
#endif

            return false;
        }
    }
}