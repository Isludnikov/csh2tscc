namespace csh2tscc;

public static class InstanceHelper
{
    public static bool InstanceOfGenericType(this Type self, Type genericType) => self.IsGenericType && self.GetGenericTypeDefinition() == genericType;

    public static bool InstanceOfGenericInterface(this Type self, Type interfaceType) => self.InstanceOfGenericType(interfaceType) || self.GetInterfaces().Any(i => i.InstanceOfGenericType(interfaceType));
}