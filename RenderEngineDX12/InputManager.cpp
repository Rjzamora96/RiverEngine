#include "pch.h"
#include "InputManager.h"
#include "Input.h"
std::unique_ptr<DirectX::Keyboard> InputManager::m_keyboard;

void InputManager::Initialize()
{
	m_keyboard = std::make_unique<DirectX::Keyboard>();
	RiverEngine::Input::keyDown = &InputManager::CheckKeyDown;
}

bool InputManager::CheckKeyDown(int key)
{
	auto kb = m_keyboard->GetState();
	return kb.IsKeyDown(DirectX::Keyboard::Keys(key));
}
