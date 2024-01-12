vec4 gaussianBlur(sampler2D source, vec2 uv, vec2 power) {
  vec4 sum = vec4(0.0);

  vec2 p = power;
  if (0.0 < power.x) {
    p.y = 0.0;
  } else {
    p.x = 0.0;
  }

  sum += texture2D(source, uv - 4.0 * p) * 0.05100;
  sum += texture2D(source, uv - 3.0 * p) * 0.09180;
  sum += texture2D(source, uv - 2.0 * p) * 0.12245;
  sum += texture2D(source, uv - 1.0 * p) * 0.15310;
  sum += texture2D(source, uv + 0.0 * p) * 0.16330;
  sum += texture2D(source, uv + 1.0 * p) * 0.15310;
  sum += texture2D(source, uv + 2.0 * p) * 0.12245;
  sum += texture2D(source, uv + 3.0 * p) * 0.09180;
  sum += texture2D(source, uv + 4.0 * p) * 0.05100;

  return sum;
}