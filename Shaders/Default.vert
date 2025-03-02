#version 330 core
layout (location = 0) in vec3 aPosition; //vertex coordinate
layout (location = 1) in vec2 aTexCoord; //Texture coordinate

out vec2 texCoord;

void main() 
{
	gl_Position = vec4(aPosition, 1.0); //coordinates
	texCoord = aTexCoord;
}
