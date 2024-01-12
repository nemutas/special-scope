import * as THREE from 'three'
import { Three } from './core/Three'
import { loadTextures } from './loader'
import { ChromaticAberration } from './offscreen/ChromaticAberration'
import { Offscreen } from './offscreen/Offscreen'
import fragmentShader from './shader/screen.fs'
import vertexShader from './shader/screen.vs'
import { HorizontalBlur } from './offscreen/HorizontalBlur'
import { VerticalBlur } from './offscreen/VerticalBlur'
import { Vignette } from './offscreen/Vignette'
import { BaseImage } from './offscreen/BaseImage'

export class Canvas extends Three {
  private screen!: THREE.Mesh<THREE.PlaneGeometry, THREE.RawShaderMaterial, THREE.Object3DEventMap>
  private offscreens: Offscreen[] = []

  constructor(canvas: HTMLCanvasElement) {
    super(canvas)

    document.body.style.setProperty('cursor', 'none')

    loadTextures('image1.webp', 'image2.webp', 'image3.webp').then((textures) => {
      this.screen = this.createPasses(textures)
      window.addEventListener('resize', this.resize.bind(this))
      this.renderer.setAnimationLoop(this.anime.bind(this))
    })
  }

  private createPasses(textures: THREE.Texture[]) {
    const baseImage = new BaseImage(this, textures)
    const chromaticAberration = new ChromaticAberration(this, baseImage.texture)
    const horizontalBlur = new HorizontalBlur(this, chromaticAberration.texture)
    const verticalBlur = new VerticalBlur(this, horizontalBlur.texture)
    const vignette = new Vignette(this, verticalBlur.texture)
    this.offscreens.push(baseImage, chromaticAberration, horizontalBlur, verticalBlur, vignette)

    return this.createOutput(vignette.texture)
  }

  private createOutput(source: THREE.Texture) {
    const geometry = new THREE.PlaneGeometry(2, 2)
    const material = new THREE.RawShaderMaterial({
      uniforms: {
        tDiffuse: { value: source },
        uResolution: { value: [this.size.width, this.size.height] },
      },
      vertexShader,
      fragmentShader,
    })
    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)

    return mesh
  }

  private resize() {
    this.offscreens.forEach((o) => o.resize())
    this.screen.material.uniforms.uResolution.value = [this.size.width, this.size.height]
  }

  private anime() {
    this.updateTime()

    this.offscreens.forEach((o) => o.render())
    this.render()
  }
}
