import gsap from 'gsap'
import * as THREE from 'three'
import { Canvas } from '../Canvas'
import { mouse2d } from '../Mouse2D'
import fragmentShader from '../shader/baseImage.fs'
import vertexShader from '../shader/screen.vs'
import { Offscreen } from './Offscreen'

export class BaseImage extends Offscreen {
  private static sort(sources: THREE.Texture[]) {
    let count = 1
    let result: THREE.Texture[] = []
    while (count <= sources.length) {
      result.push(sources.find((s) => s.name.endsWith(count.toString()))!)
      count++
    }
    return result
  }

  private zoomTarget = 0.5
  private mouseTarget = [0, 0]
  private srcImages: THREE.Texture[]
  private imageCounter = 0

  constructor(canvas: Canvas, sources: THREE.Texture[]) {
    const srcImages = BaseImage.sort(sources)

    const material = new THREE.RawShaderMaterial({
      uniforms: {
        uCurrent: {
          value: {
            source: srcImages[0],
            coveredScale: canvas.coveredScale(srcImages[0].userData.aspect),
          },
        },
        uNext: {
          value: {
            source: srcImages[1],
            coveredScale: canvas.coveredScale(srcImages[1].userData.aspect),
          },
        },
        uMouse: { value: mouse2d.position },
        uZoom: { value: 0.5 },
        uProgress: { value: 0 },
      },
      vertexShader,
      fragmentShader,
    })

    super(canvas, material)
    this.srcImages = srcImages
    this.addEvents()

    this.createImageLoopAnimation()
  }

  addEvents() {
    window.addEventListener('wheel', (e) => {
      this.zoomTarget += 0 < e.deltaY ? 0.03 : -0.03
      this.zoomTarget = THREE.MathUtils.clamp(this.zoomTarget, 0.2, 0.8)
    })

    window.addEventListener('mousemove', () => {
      this.mouseTarget = mouse2d.position
    })
  }

  private updateCoveredScale() {
    this.uniforms.uCurrent.value.coveredScale = this.canvas.coveredScale(this.uniforms.uCurrent.value.source.userData.aspect)
    this.uniforms.uNext.value.coveredScale = this.canvas.coveredScale(this.uniforms.uNext.value.source.userData.aspect)
  }

  private createImageLoopAnimation() {
    gsap.to(this.uniforms.uProgress, {
      value: 1,
      duration: 1.5,
      ease: 'power4.out',
      delay: 15,
      repeat: -1,
      repeatDelay: 15,
      onRepeat: () => {
        this.imageCounter++
        this.uniforms.uProgress.value = 0
        this.uniforms.uCurrent.value.source = this.srcImages[this.imageCounter % this.srcImages.length]
        this.uniforms.uNext.value.source = this.srcImages[(this.imageCounter + 1) % this.srcImages.length]
        this.updateCoveredScale()
      },
    })
  }

  resize() {
    super.resize()
    this.updateCoveredScale()
  }

  render() {
    this.uniforms.uMouse.value[0] = THREE.MathUtils.lerp(this.uniforms.uMouse.value[0], this.mouseTarget[0], 0.2)
    this.uniforms.uMouse.value[1] = THREE.MathUtils.lerp(this.uniforms.uMouse.value[1], this.mouseTarget[1], 0.2)
    this.uniforms.uZoom.value = THREE.MathUtils.lerp(this.uniforms.uZoom.value, this.zoomTarget, 0.15)
    super.render()
  }
}
