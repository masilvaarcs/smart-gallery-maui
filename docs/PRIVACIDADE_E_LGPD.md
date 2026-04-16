# Privacidade e LGPD - Smart Gallery MAUI

## Escopo

Este documento cobre o comportamento de privacidade no cliente Smart Gallery MAUI.

## Permissoes de dispositivo

- Camera: usada apenas para captura de imagem quando o usuario escolhe essa acao.
- Biblioteca de fotos: usada para selecionar imagem para upload.

## Regras no app

- Antes do upload, o app solicita confirmacao explicita de privacidade e direitos de uso.
- Mensagens de erro para usuario final nao exibem detalhes tecnicos internos.

## Responsabilidade do usuario

Ao confirmar o upload, o usuario declara que:

- possui autorizacao para uso da imagem;
- nao esta publicando conteudo com dados pessoais sem base legal;
- entende que o backend pode processar tags por IA (Rekognition).

## Recomendacoes

- Evite imagens com dados sensiveis visiveis.
- Revogue ou exclua conteudo indevido imediatamente.
- Utilize sempre endpoint HTTPS em ambiente produtivo.
