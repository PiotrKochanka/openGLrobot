#version 330

uniform mat4 P;
uniform mat4 V;
uniform mat4 M;

out vec4 pixelColor; //Zmienna wyjsciowa fragment shadera. Zapisuje sie do niej ostateczny (prawie) kolor piksela

in vec4 iColor;
in vec4 iVertex;
in vec4 iNormal;
in vec2 itexCoord;
uniform vec4 zm;

uniform sampler2D texunit;

void main(void) {

	vec4 l = normalize(V * zm - V * M * iVertex);

	vec4 n = normalize(V * M * iNormal);

	float nl = clamp(dot(n, l), 0, 1);

	vec4 texColor = texture(texunit, itexCoord);

	vec4 txC = texColor;

	vec4 Ka = vec4(0, 0, 0, 0);
	vec4 Kd = vec4(1, 1, 1, 1);
	vec4 La = vec4(0, 0, 0, 0);
	vec4 Ld = vec4(1, 1, 1, 1);

	pixelColor = vec4(txC.rgb * Kd.rgb * Ld.rgb * nl + Ka.rgb * La.rgb, iColor.a);
}
