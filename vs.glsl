#version 330

//Zmienne jednorodne
uniform mat4 P;
uniform mat4 V;
uniform mat4 M;

//Atrybuty
in vec4 a_v; //wspolrzedne wierzcholka w przestrzeni modelu
in vec4 color;
in vec4 a_n;
in vec2 a_t;

out vec4 iVertex;
out vec4 iNormal;
out vec4 iColor;
out vec2 itexCoord;

void main(void) {
    iVertex = a_v;
    iNormal = a_n;
    iColor = color;
    itexCoord = a_t;

    gl_Position=P*V*M*a_v;
}
