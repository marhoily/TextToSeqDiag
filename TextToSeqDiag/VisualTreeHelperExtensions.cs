using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using JetBrains.Annotations;

namespace TextToSeqDiag
{
    [PublicAPI]
    public static class VisualTreeHelperExtensions
    {
        public static IEnumerable<T> GetAllVisualAncestors<T>(this DependencyObject root)
            where T : DependencyObject
        {
            var i = root;
            while (i != null)
            {
                var r = i as T;
                if (r != null) yield return r;
                i = VisualTreeHelper.GetParent(i);
            }
        }

        public static IEnumerable<DependencyObject> GetAllVisualAncestors(this DependencyObject root)
        {
            var i = root;
            while (i != null)
            {
                yield return i;
                i = VisualTreeHelper.GetParent(i);
            }
        }

        public static IEnumerable<DependencyObject> GetAllVisualDescendants(this DependencyObject root)
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                foreach (var cc in child.GetAllVisualDescendants())
                    yield return cc;
                yield return child;
            }
        }

        public static IEnumerable<T> GetAllVisualDescendants<T>(this DependencyObject root)
            where T : DependencyObject
        {
            return root.GetAllVisualDescendants().OfType<T>();
        }

        public static T GetNearestVisualAncestor<T>(this DependencyObject root)
            where T : DependencyObject
        {
            var i = root;
            while (i != null)
            {
                var r = i as T;
                if (r != null) return r;
                i = VisualTreeHelper.GetParent(i);
            }
            return null;
        }

        public static T GetNearestTemplatedParent<T>(this DependencyObject root)
            where T : class
        {
            return root.GetAllVisualAncestors<FrameworkElement>()
                .Where(p => p.TemplatedParent != null)
                .Select(p => p.TemplatedParent as T)
                .FirstOrDefault();
        }

        public static DependencyObject GetNearestTemplatedParent(this DependencyObject root)
        {
            return root.GetAllVisualAncestors<FrameworkElement>()
                .Where(p => p.TemplatedParent != null)
                .Select(p => p.TemplatedParent)
                .FirstOrDefault();
        }

        public static T VisualRootOrNull<T>(this DependencyObject reference)
            where T : DependencyObject
        {
            for (var i = reference; i != null; i = VisualTreeHelper.GetParent(reference))
                reference = i;
            return reference as T;
        }

        public static T VisualRoot<T>(this DependencyObject reference)
            where T : DependencyObject
        {
            for (var i = reference; i != null; i = GetParent(reference))
                reference = i;
            return reference as T;
        }

        private static DependencyObject GetParent(DependencyObject reference)
        {
            var result = VisualTreeHelper.GetParent(reference);
            if (result != null) return result;
            var frameworkElement = reference as FrameworkElement;
            if (frameworkElement == null) return null;
            return frameworkElement.Parent ?? frameworkElement.TemplatedParent;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    var children = child as T;
                    if (children != null)
                        yield return children;
                    foreach (var obj in FindVisualChildren<T>(child))
                        yield return obj;
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (var depObj1 in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
                {
                    var obj1 = depObj1 as T;
                    if (obj1 != null)
                        yield return obj1;
                    foreach (var obj in FindLogicalChildren<T>(depObj1))
                        yield return obj;
                }
            }
        }

        public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
        {
            var dependencyObject1 = initial;
            var dependencyObject2 = initial;
            for (;
                dependencyObject1 != null;
                dependencyObject1 =
                    dependencyObject1 is Visual || dependencyObject1 is Visual3D
                        ? VisualTreeHelper.GetParent(dependencyObject1)
                        : LogicalTreeHelper.GetParent(dependencyObject1))
                dependencyObject2 = dependencyObject1;
            return dependencyObject2;
        }

        public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            var reference = dependencyObject;
            do
            {
                reference = VisualTreeHelper.GetParent(reference);
            } while (reference != null && !(reference is T));
            return reference as T;
        }

        public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            var current = dependencyObject;
            do
            {
                var reference = current;
                current = LogicalTreeHelper.GetParent(current) ?? VisualTreeHelper.GetParent(reference);
            } while (current != null && !(current is T));
            return current as T;
        }
    }
}