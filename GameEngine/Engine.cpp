#include "Engine.h"

#include "Vector2.h"
#include "Component.h"
#include "Transform.h"

#include "LuaBridge.h"
#include "RefCountedPtr.h"
extern "C" {
# include "lua.h"
# include "lauxlib.h"
# include "lualib.h"
}

using namespace luabridge;

Engine::Engine()
{
}


Engine::~Engine()
{
}

bool Engine::Initialize()
{
	luabridge::lua_State* L = luaL_newstate();
	luaL_openlibs(L);
	getGlobalNamespace(L)
		.beginClass<Vector2>("Vector2")
		.addConstructor<void(*)(void), RefCountedPtr<Vector2>>()
		.addData("x", &Vector2::x)
		.addData("y", &Vector2::y)
		.endClass();
	getGlobalNamespace(L)
		.beginClass<Component>("Component")
		.addConstructor<void(*)(void)>()
		.endClass()
		.deriveClass<Transform, Component>("Transform")
		.addProperty("rotation", &Transform::getRotation, &Transform::setRotation)
		.addProperty("position", &Transform::getPosition, &Transform::setPosition)
		.endClass();
	return true;
}
