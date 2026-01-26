# 反射

>参考：https://blog.csdn.net/xoyojank/article/details/7015001

- .net本身也支持反射, 问题是如何反射到C++里。
- 用mono api取得类的meta信息, 然后反应到编辑器上去编辑。
- 如果想偷懒的话, 可以直接用mono创建个窗口, 放个propertygrid控件, 再这个窗口嵌入到主编辑器的界面。


代码：
```CPP
int depth = 0;
void ListFields(MonoObject* object)
{
	assert(NULL != object);
	++depth;
 
	MonoClass* objectClass = mono_object_get_class(object);
	const char* className = mono_class_get_name(objectClass);
	printf("[%s]\n", className);
	void* iter = NULL;
	MonoClassField* field = NULL;
	while (field = mono_class_get_fields(objectClass, &iter))
	{
		for (int i = 0; i < depth; ++i)
		{
			printf("\t");
		}
		const char* fieldName = mono_field_get_name(field);
		printf("%s : ", fieldName);
		MonoType* fieldType = mono_field_get_type(field);
		int type = mono_type_get_type(fieldType);
		switch (type)
		{
		case MONO_TYPE_BOOLEAN:
			{
				bool boolValue = false;
				mono_field_get_value(object, field, &boolValue);
				printf("%s\n", boolValue ? "true" : "false");
			}
			break;
		case MONO_TYPE_I4:
			{
				int intValue = 0;
				mono_field_get_value(object, field, &intValue);
				printf("%d\n", intValue);
			}
			break;
		case MONO_TYPE_R4:
			{
				float floatValue = 0;
				mono_field_get_value(object, field, &floatValue);
				printf("%f\n", floatValue);
			}
			break;
		case MONO_TYPE_CHAR:
		case MONO_TYPE_I1:
		case MONO_TYPE_U1:
		case MONO_TYPE_I2:
		case MONO_TYPE_U2:
		case MONO_TYPE_U4:
		case MONO_TYPE_I:
		case MONO_TYPE_U:
		case MONO_TYPE_I8:
		case MONO_TYPE_U8:
		case MONO_TYPE_R8:
			{
				//@todo
			}
			break;
		case MONO_TYPE_SZARRAY:
			{
				MonoObject* value = mono_field_get_value_object(mono_domain_get(), field, object);
				MonoArray* array = (MonoArray*)value;
				uintptr_t size = mono_array_length(array);
				MonoClass* elementClass = mono_class_get_element_class(mono_object_get_class(value));
				MonoType* elementType = mono_class_get_type(elementClass);
				switch (mono_type_get_type(elementType))
				{
				case MONO_TYPE_BOOLEAN:
					{
						bool* data = mono_array_addr(array, bool, 0);
						for (int i = 0; i < size; ++i)
						{
							printf("%s ", data[i] ? "true" : "false");
						}
						printf("\n");
					}
					break;
				default: //@todo
					break;
				}
				className = mono_class_get_name(elementClass);
			}
			break;
		case MONO_TYPE_STRING:
			{
				MonoObject* value = mono_field_get_value_object(mono_domain_get(), field, object);
				MonoString* valueString = (MonoString*)value;
				const char* valueCStr = mono_string_to_utf8(valueString);
				printf("%s\n", valueCStr);
			}
			break;
		default:
			{
				MonoObject* value = mono_field_get_value_object(mono_domain_get(), field, object);
				ListFields(value);
			}
			break;
		}
	}
	--depth;
}
```