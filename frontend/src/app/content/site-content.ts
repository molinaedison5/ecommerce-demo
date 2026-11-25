/**
 * Public copy for Centro K-9.
 * Prices, hours, illustrations, testimonials, the map pin and `siteOrigin` are examples.
 * Replace them here. The WhatsApp number lives only in `businessPhone`.
 */

export const businessPhone = {
  e164: '+593984204476',
  display: '+593 98 420 4476',
} as const;

/** Example origin for canonical and Open Graph URLs. Replace with the real domain. */
export const siteOrigin = 'https://k9-latacunga.example';

export const exampleNotice =
  'Precios, horario, ilustraciones y testimonios son contenido de ejemplo. Sustitúyelos antes de publicar.';

export interface SiteService {
  slug: string;
  name: string;
  summary: string;
  priceFrom: string;
  priceDetail: string;
  includes: readonly string[];
  requirements: readonly string[];
  whatsappMessage: string;
  seoTitle: string;
  seoDescription: string;
  catalogLink: boolean;
}

export interface GalleryPair {
  id: string;
  title: string;
  beforeSrc: string;
  afterSrc: string;
  beforeAlt: string;
  afterAlt: string;
  caption: string;
}

export interface ExampleTestimonial {
  id: string;
  quote: string;
  attribution: string;
}

export interface ExampleHours {
  label: string;
  value: string;
  schemaDays: readonly string[];
  opens: string | null;
  closes: string | null;
}

export const siteContent = {
  brand: {
    name: 'Centro de Adiestramiento & Cuidado Animal K-9',
    shortName: 'Centro K-9',
    slogan: 'Perros felices, familias tranquilas',
    hook: 'Agenda tu clase GRATIS',
    proposal:
      'Ayudamos a mejorar la convivencia entre humanos y perros, con adiestramiento y educación canina basada en el respeto.',
    experience: 'Más de 6 años de experiencia.',
  },
  address: {
    line: 'Parroquia San Buenaventura, a 100 m del Subcentro de Salud',
    locality: 'Latacunga',
    region: 'Cotopaxi',
    country: 'Ecuador',
    countryCode: 'EC',
  },
  seo: {
    home: {
      title: 'Adiestramiento canino en Latacunga | Centro K-9',
      description:
        'Adiestramiento canino y guardería canina en Latacunga, parroquia San Buenaventura. Agenda tu clase gratis. Perros felices, familias tranquilas.',
      path: '/',
    },
    services: {
      title: 'Servicios caninos en Latacunga | Centro K-9',
      description:
        'Guardería canina, hospedaje, baños y adiestramiento canino en Latacunga. Precios de ejemplo desde el primer día y clase de evaluación gratis.',
      path: '/servicios',
    },
    freeClass: {
      title: 'Agenda tu clase gratis de adiestramiento canino | Centro K-9',
      description:
        'Pide la clase gratis de adiestramiento canino en Latacunga. Cuéntanos la raza, la edad y el problema de conducta, o continúa por WhatsApp.',
      path: '/clase-gratis',
    },
    gallery: {
      title: 'Antes y después del adiestramiento canino | Centro K-9',
      description:
        'Ilustraciones de ejemplo de convivencia antes y después del adiestramiento canino en Latacunga. Las fotos reales todavía no están publicadas.',
      path: '/galeria',
    },
    about: {
      title: 'Sobre el Centro K-9 en San Buenaventura, Latacunga',
      description:
        'Centro de adiestramiento y cuidado animal K-9 en la parroquia San Buenaventura, Latacunga, Cotopaxi. Más de 6 años junto a familias y perros.',
      path: '/nosotros',
    },
    catalog: {
      title: 'Productos de entrenamiento canino en Latacunga | Centro K-9',
      description:
        'Productos de ejemplo para el entrenamiento canino, con retiro en el local del Centro K-9 en San Buenaventura, Latacunga.',
      path: '/catalog',
    },
    cart: {
      title: 'Carrito | Centro K-9',
      description: 'El carrito y el pago no forman parte de esta fase del sitio.',
      path: '/cart',
    },
    account: {
      title: 'Cuenta | Centro K-9',
      description: 'El acceso con JWT ya existe. La pantalla para revisar solicitudes llega después.',
      path: '/account',
    },
  },
  whatsapp: {
    defaultMessage:
      'Hola, escribo desde la web del Centro K-9 en Latacunga. Quiero información.',
    freeClassMessage: 'Hola, quiero agendar la clase GRATIS de adiestramiento canino.',
  },
  services: [
    {
      slug: 'guarderia-canina',
      name: 'Guardería canina',
      summary: 'Cuidado de día para que el perro no se quede solo mientras la familia trabaja o estudia.',
      priceFrom: 'desde $12',
      priceDetail: 'por día. Precio de ejemplo.',
      includes: [
        'Estadía durante la jornada de ejemplo (08:00–18:00)',
        'Agua fresca y un descanso a la sombra',
        'Salidas cortas al patio',
        'Nota breve al momento del retiro',
      ],
      requirements: ['Vacunas al día', 'Desparasitación'],
      whatsappMessage:
        'Hola, quiero información sobre la guardería canina en Latacunga (Centro K-9).',
      seoTitle: 'Guardería canina en Latacunga | Centro K-9',
      seoDescription:
        'Guardería canina en Latacunga, parroquia San Buenaventura. Cuidado de día con precio de ejemplo desde $12. Se piden vacunas al día y desparasitación.',
      catalogLink: false,
    },
    {
      slug: 'hospedaje-canino',
      name: 'Hospedaje canino',
      summary: 'Alojamiento cuando la familia viaja. El perro duerme en el centro, no en una jaula de paso.',
      priceFrom: 'desde $18',
      priceDetail: 'por noche. Precio de ejemplo.',
      includes: [
        'Noche de alojamiento',
        'Paseos de ejemplo por la mañana y por la tarde',
        'Alimentación con la comida que deja la familia',
        'Aviso por WhatsApp si algo cambia',
      ],
      requirements: ['Vacunas al día', 'Desparasitación'],
      whatsappMessage: 'Hola, quiero información sobre el hospedaje canino en el Centro K-9.',
      seoTitle: 'Hospedaje canino en Latacunga | Centro K-9',
      seoDescription:
        'Hospedaje canino en Latacunga para cuando la familia viaja. Precio de ejemplo desde $18 la noche. Requisitos: vacunas al día y desparasitación.',
      catalogLink: false,
    },
    {
      slug: 'banos-caninos',
      name: 'Baños caninos',
      summary: 'Baño tranquilo, sin prisa, para perros que necesitan volver a casa limpios y secos.',
      priceFrom: 'desde $10',
      priceDetail: 'por baño. Precio de ejemplo.',
      includes: [
        'Baño con agua tibia',
        'Secado',
        'Corte básico de uñas',
        'Revisión de orejas, sin tratamiento médico',
      ],
      requirements: [],
      whatsappMessage: 'Hola, quiero información sobre los baños caninos del Centro K-9.',
      seoTitle: 'Baños caninos en Latacunga | Centro K-9',
      seoDescription:
        'Baños caninos en Latacunga, a 100 m del Subcentro de Salud de San Buenaventura. Precio de ejemplo desde $10. También hay guardería y adiestramiento.',
      catalogLink: false,
    },
    {
      slug: 'adiestramiento-canino',
      name: 'Adiestramiento y educación canina',
      summary:
        'Clases para mejorar la convivencia en casa y en la calle. La primera evaluación es la clase gratis.',
      priceFrom: 'desde $25',
      priceDetail: 'por sesión, después de la clase gratis. Precio de ejemplo.',
      includes: [
        'Clase de evaluación gratis',
        'Ejercicios de convivencia basados en el respeto',
        'Pautas para la familia, no solo para el perro',
        'Seguimiento de un problema concreto (correa, ladridos, quedarse solo)',
      ],
      requirements: [],
      whatsappMessage: 'Hola, quiero agendar la clase GRATIS de adiestramiento canino en Latacunga.',
      seoTitle: 'Adiestramiento canino en Latacunga | Centro K-9',
      seoDescription:
        'Adiestramiento canino en Latacunga basado en el respeto. Agenda la clase gratis y cuéntanos el problema de conducta. Sesiones de ejemplo desde $25.',
      catalogLink: false,
    },
    {
      slug: 'productos-de-entrenamiento',
      name: 'Productos de entrenamiento',
      summary: 'Correas, premios y equipo de ejemplo. Se retiran en el local; esta fase no cobra en línea.',
      priceFrom: 'según catálogo',
      priceDetail: 'Precios de ejemplo del catálogo. Retiro en el local.',
      includes: [
        'Retiro en San Buenaventura',
        'Equipo de ejemplo para las clases',
        'Sin envío y sin pago en línea en esta fase',
      ],
      requirements: [],
      whatsappMessage:
        'Hola, quiero consultar productos de entrenamiento para retirarlos en el Centro K-9.',
      seoTitle: 'Productos de entrenamiento canino en Latacunga | Centro K-9',
      seoDescription:
        'Venta de productos de entrenamiento canino con retiro en el local del Centro K-9, parroquia San Buenaventura, Latacunga. El catálogo muestra precios de ejemplo.',
      catalogLink: true,
    },
  ] as const satisfies readonly SiteService[],
  gallery: [
    {
      id: 'correa',
      title: 'La correa',
      beforeSrc: '/media/gallery-leash-before.svg',
      afterSrc: '/media/gallery-leash-after.svg',
      beforeAlt: 'Ilustración de ejemplo: un perro tira de la correa hacia adelante.',
      afterAlt: 'Ilustración de ejemplo: el mismo perro camina al lado, con la correa floja.',
      caption: 'Ilustración de ejemplo. No es una foto de un alumno real.',
    },
    {
      id: 'quedarse',
      title: 'Quedarse',
      beforeSrc: '/media/gallery-jump-before.svg',
      afterSrc: '/media/gallery-jump-after.svg',
      beforeAlt: 'Ilustración de ejemplo: un perro salta sobre una persona en la puerta.',
      afterAlt: 'Ilustración de ejemplo: el perro espera sentado junto a la persona.',
      caption: 'Ilustración de ejemplo. Sustitúyela por una foto de antes y después.',
    },
    {
      id: 'casa',
      title: 'En casa',
      beforeSrc: '/media/gallery-home-before.svg',
      afterSrc: '/media/gallery-home-after.svg',
      beforeAlt: 'Ilustración de ejemplo: un perro ladra frente a la puerta.',
      afterAlt: 'Ilustración de ejemplo: el perro descansa echado junto a la puerta.',
      caption: 'Ilustración de ejemplo, dibujada para esta fase del sitio.',
    },
  ] satisfies readonly GalleryPair[],
  testimonials: [
    {
      id: 'ejemplo-1',
      quote:
        'Texto de ejemplo: la clase nos dio una pauta clara para los paseos, sin gritos y sin prisa.',
      attribution: 'Familia de ejemplo',
    },
    {
      id: 'ejemplo-2',
      quote:
        'Texto de ejemplo: dejamos a nuestro perro en la guardería y al recogerlo venía cansado, no alterado.',
      attribution: 'Tutor de ejemplo',
    },
    {
      id: 'ejemplo-3',
      quote:
        'Texto de ejemplo: el hospedaje nos sirvió para un viaje corto. Confirmamos vacunas antes de dejarlo.',
      attribution: 'Familia de ejemplo',
    },
  ] satisfies readonly ExampleTestimonial[],
  hours: [
    {
      label: 'Lunes a viernes',
      value: '08:00–18:00',
      schemaDays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
      opens: '08:00',
      closes: '18:00',
    },
    {
      label: 'Sábado',
      value: '08:00–13:00',
      schemaDays: ['Saturday'],
      opens: '08:00',
      closes: '13:00',
    },
    {
      label: 'Domingo',
      value: 'Cerrado',
      schemaDays: ['Sunday'],
      opens: null,
      closes: null,
    },
  ] satisfies readonly ExampleHours[],
  map: {
    title: 'Mapa de ejemplo: Subcentro de Salud, San Buenaventura, Latacunga',
    caption:
      'Pin de ejemplo sobre el Subcentro de Salud de San Buenaventura. El local está a unos 100 m; sustituye la coordenada cuando la confirmes.',
    embedUrl:
      'https://www.openstreetmap.org/export/embed.html?bbox=-78.618%2C-0.898%2C-78.598%2C-0.882&layer=mapnik&marker=-0.889802%2C-78.607588',
    openUrl: 'https://www.openstreetmap.org/?mlat=-0.889802&mlon=-78.607588#map=17/-0.889802/-78.607588',
  },
  steps: [
    {
      title: 'Cuéntanos el problema',
      detail: 'Nombre, teléfono, raza, edad y qué está pasando en casa o en la calle.',
    },
    {
      title: 'Ven a la clase gratis',
      detail: 'Es una evaluación, no una promesa de resultado en una hora.',
    },
    {
      title: 'Seguimos si encaja',
      detail: 'Si la familia quiere continuar, se arma un plan. El calendario de reservas llega después.',
    },
  ],
} as const;

export interface PageSeo {
  title: string;
  description: string;
  path: string;
}

export function pageUrl(path: string): string {
  const normalized = path.startsWith('/') ? path : `/${path}`;
  return `${siteOrigin}${normalized === '/' ? '/' : normalized}`;
}

export function serviceBySlug(slug: string | null): SiteService | undefined {
  return siteContent.services.find((service) => service.slug === slug);
}
