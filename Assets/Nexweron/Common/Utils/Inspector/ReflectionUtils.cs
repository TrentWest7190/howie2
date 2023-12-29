#if UNITY_EDITOR
using System;
using System.Reflection;

namespace Nexweron.Common.Utils
{
	public static class ReflectionUtils
	{
		public static PropertyInfo FindPropByPath(ref object target, String dirPath, string propName) {
			PropertyInfo info = null;
			var parentFieldInfo = FindFieldByPath(ref target, dirPath);
			if (parentFieldInfo != null) {
				var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
				info = parentFieldInfo.FieldType.GetProperty(propName, flags);
				target = parentFieldInfo.GetValue(target);
			}
			return info;
		}

		public static PropertyInfo FindPropByPath(ref object target, String propPath) {
			var dotIndex = propPath.LastIndexOf(".", StringComparison.Ordinal);
			if (dotIndex > 0) {
				var propName = propPath.Substring(dotIndex + 1);
				var dirPath = propPath.Substring(0, dotIndex);
				return FindPropByPath(ref target, dirPath, propName);
			}
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return target.GetType().GetProperty(propPath, flags);
		}
		
		public static FieldInfo FindFieldByPath(ref object rootTarget, String path) {
			FieldInfo info = null;
			var target = rootTarget;
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			var t = target.GetType();
			var parts = path.Split('.');
			foreach (var part in parts) {
				target = info != null ? info.GetValue(target) : target;
				info = FindFieldHierarchy(t, part, flags);
				if (info == null) return null;
				t = info.FieldType;
			}
			rootTarget = target;
			return info;
		}
		
		public static FieldInfo FindFieldHierarchy(Type t, string name, BindingFlags flags) {
			FieldInfo info;
			while (t != null) {
				info = t.GetField(name, flags);
				if (info != null) return info;
				t = t.BaseType;
			}
			return null;
		}
	}
}
#endif