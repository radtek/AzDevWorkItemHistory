using System;
using AzDevWorkItemHistory.Credentials;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using NUnit.Framework;

namespace Tests
{
    public class CredentialManagerTests
    {
        [Test]
        public void CanAddCredentialToEmpty()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            var sut = new CredentialManager(new YamlFileCredentialStore(tmpFile.Path));
            sut.Add(CredentialV1.CreateFromPlainText(new Uri("https://example.org"), "user", "secret"));

            var value = sut.Get(new Uri("https://example.org"));
            Assert.That(value.IsSome, Is.True);
            Assert.That(value.ValueUnsafe().GetUri(), Is.EqualTo(new Uri("https://example.org")));
        }

        [Test]
        public void CannotAddWithDuplicateUri()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            var sut = new CredentialManager(new YamlFileCredentialStore(tmpFile.Path));
            sut.Add(CredentialV1.CreateFromPlainText(new Uri("https://example.org"), "user", "secret"));

            Assert.Throws<InvalidOperationException>(
                () => sut.Add(CredentialV1.CreateFromPlainText(new Uri("https://example.org"), "user", "secret")));
        }

        [Test]
        public void CanRemoveCredentialsByUri()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            var sut = new CredentialManager(new YamlFileCredentialStore(tmpFile.Path));

            sut.Add(CredentialV1.CreateFromPlainText(new Uri("https://example.org"), "user", "secret"));
            sut.Remove(new Uri("https://example.org"));

            Assert.That(sut.Get(new Uri("https://example.org")), Is.EqualTo(Option<CredentialV1>.None));
        }
        [Test]
        public void DoesNotHaveUriWhenItDoesnt()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            var sut = new CredentialManager(new YamlFileCredentialStore(tmpFile.Path));

            Assert.That(sut.HasUri(new Uri("https://example.org")), Is.False);
        }

        [Test]
        public void DoesHaveUriWhenItDoes()
        {
            using var tmpFile = TempFile.Init("credentials.yml");

            var sut = new CredentialManager(new YamlFileCredentialStore(tmpFile.Path));
            sut.Add(CredentialV1.CreateFromPlainText(new Uri("https://example.org"), "user", "secret"));

            Assert.That(sut.HasUri(new Uri("https://example.org")), Is.True);
        }

    }
}