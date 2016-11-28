#include "Engine.h"

#include "Vector2.h"
#include "Component.h"
#include "Transform.h"
#include "Sprite.h"
#include "Input.h"

#include "LuaBridge.h"
#include "RefCountedPtr.h"
#include "Scene.h"
#include <Windows.h>
extern "C" {
# include "lua.h"
# include "lauxlib.h"
# include "lualib.h"
}

#undef SendMessage;

using namespace luabridge;

namespace RiverEngine
{

	Engine::Engine()
	{
	}


	Engine::~Engine()
	{
	}
	using namespace RiverEngine;
	bool Engine::Init()
	{
		wchar_t currDir[MAX_PATH];
		GetCurrentDirectoryW(MAX_PATH, currDir);
		std::string s = "\\Assets\\";
		std::wstring stemp = currDir + std::wstring(s.begin(), s.end());
		LPCWSTR sw = stemp.c_str();
		SetCurrentDirectoryW(sw);
		luabridge::lua_State* L = luaL_newstate();
		luaL_openlibs(L);
		getGlobalNamespace(L)
			.beginClass<Vector2>("Vector2")
			.addConstructor<void(*)(void)>()
			.addData("x", &Vector2::x)
			.addData("y", &Vector2::y)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Input>("Input")
			.addStaticFunction("IsKeyDown", &Input::IsKeyDown)
			.addStaticFunction("IsKeyPressed", &Input::IsKeyPressed)
			.addStaticFunction("IsKeyReleased", &Input::IsKeyReleased)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Component>("Component")
			.addConstructor<void(*)(void)>()
			.addStaticFunction("Property", &Component::Property)
			.addData("entity", &Component::owner)
			.endClass()
			.deriveClass<Transform, Component>("Transform")
			.addData("rotation", &Transform::localRotation)
			.addData("scale", &Transform::localScale)
			.addData("position", &Transform::localPosition)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Scene>("Scene")
			.addStaticFunction("ChangeScene", &Scene::ChangeScene)
			.addStaticFunction("Instantiate", &Scene::Instantiate)
			.addStaticFunction("GetEntityByTag", &Scene::GetEntityByTag)
			.addStaticFunction("GetEntities", &Scene::GetEntities)
			.endClass();
		getGlobalNamespace(L)
			.deriveClass<Sprite, Component>("Sprite")
			.addProperty("image", &Sprite::GetSprite, &Sprite::SetSprite)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Entity>("Entity")
			.addConstructor<void(*)(void)>()
			.addData("transform", &Entity::transform)
			.addData("sprite", &Entity::sprite)
			.addFunction("getComponent", &Entity::GetComponent)
			.addFunction("sendMessage", &Entity::SendMessage)
			.addFunction("hasTag", &Entity::HasTag)
			.endClass();
		Component::AssignState(L);
		Entity::AssignState(L);
		Scene::AssignState(L);
		Input::InitializeBindings();
		if (luaL_dofile(L, "Sprites.assets") == 0)
		{
			LuaRef table = getGlobal(L, "sprites");
			int tableLen = table.length();
			for (int i = 1; i <= table.length(); ++i)
			{
				Sprite::addTexture(table[i].cast<std::string>());
			}
		}
		if (luaL_dofile(L, "Properties.assets") == 0)
		{
			LuaRef table = getGlobal(L, "properties");
			Scene* scene = new Scene();
			Scene::SetActiveScene(scene);
			scene->LoadSceneFromFile(table["startScene"].cast<std::string>());
		}
		return true;
	}

}