#include "pch.h"
#include "InputManager.h"
#include "Input.h"

std::unique_ptr<DirectX::Keyboard> InputManager::m_keyboard;
DirectX::Keyboard::KeyboardStateTracker InputManager::m_tracker;

void InputManager::Initialize()
{
	m_keyboard = std::make_unique<DirectX::Keyboard>();
	RiverEngine::Input::keyDown = &InputManager::CheckKeyDown;
	RiverEngine::Input::keyPressed = &InputManager::CheckKeyPressed;
	RiverEngine::Input::keyReleased = &InputManager::CheckKeyReleased;
}

bool InputManager::CheckKeyDown(int key)
{
	auto kb = m_keyboard->GetState();
	return kb.IsKeyDown(DirectX::Keyboard::Keys(key));
}

bool InputManager::CheckKeyPressed(int key)
{
	return m_tracker.IsKeyPressed(DirectX::Keyboard::Keys(key));
}

bool InputManager::CheckKeyReleased(int key)
{
	return m_tracker.IsKeyReleased(DirectX::Keyboard::Keys(key));
}

void InputManager::Update()
{
	m_tracker.Update(m_keyboard->GetState());
}
