﻿using Xunit;

namespace JsonPathConverter.ColumnMapper.NewObject.Test
{
    public class JsonTemplateConvertObject
    {
        [Fact]
        public void JsonTemplate()
        {
            string jsonTemplate = "{\"contacts\":[{\"phone\":\"$.phone\",\"tel\":\"$.tel\"}],\"id\":\"Guid\",\"name\":\"$.name\",\"createDataRange\":{\"start\":\"$.startCreateData\",\"end\":\"$.endCreateData\"},\"hyperLinkContext\":\"$.hyperLink.context\",\"hyperLinkUrl\":\"$.hyperLink.url\",\"departments\":[{\"id\":\"$.department.id\",\"name\":\"$.department.name\"}],\"leaders\":[\"$.leaders.name\"],\"leaderNames\":[{\"name\":\"$.leaders.name\",\"remark\":\"leader\"}],\"role\":{\"id\":\"$.roles[0].id\",\"name\":\"$.roles[0].name\"},\"phone1\":\"$.phone[0]\",\"phone2\":\"$.phone[1]\",\"suggests\":[\"$.define.suggest.message\"],\"suggest1\":\"$.define.suggest.message[0]\",\"suggest2\":\"$.define.suggest.message[1]\",\"suggest3\":\"$.define.suggest.message[2]\"}";

            string jsonSource = "{\"name\":\"azir\",\"startCreateData\":\"2022-10-13 00:00:00\",\"endCreateData\":\"2022-10-14 00:00:00\",\"hyperLink\":{\"context\":\"百度一下\",\"url\":\"https://www.baidu.com\"},\"department\":{\"id\":\"111\",\"name\":\"研发部\"},\"leaders\":[{\"name\":\"azirliang\"},{\"name\":\"azir\"}],\"roles\":[{\"id\":\"2222\",\"name\":\"role2222\"}],\"phone\":[\"10086\",\"10010\",\"10001\"],\"tel\":[\"111\",\"112\",\"113\",\"114\"],\"define\":{\"suggest\":{\"message\":[\"aaa\",\"bbb\"]}}}";

            var obj = new ColumnMapperNewObject().MapToObjectByTemplate(jsonTemplate, jsonSource);

            var contacts = obj.Data!["contacts"] as List<IDictionary<string, object>>;

            Assert.True(contacts!.Count == 4);

            Assert.Equal("111", contacts[0]["tel"]);
            Assert.Equal("112", contacts[1]["tel"]);
            Assert.Equal("113", contacts[2]["tel"]);
            Assert.Equal("114", contacts[3]["tel"]);

            Assert.Equal("10086", contacts[0]["phone"]);
            Assert.Equal("10010", contacts[1]["phone"]);
            Assert.Equal("10001", contacts[2]["phone"]);

            var leaderNames = obj.Data!["leaderNames"] as List<IDictionary<string, object>>;

            Assert.Equal("azirliang", leaderNames![0]["name"]);
            Assert.Equal("leader", leaderNames![0]["remark"]);
            Assert.Equal("azir", leaderNames[1]!["name"]);
            Assert.Equal("leader", leaderNames[1]!["remark"]);

            Assert.Equal("azir", obj.Data!["name"]);

            var createDataRange = obj.Data!["createDataRange"] as IDictionary<string, object>;

            Assert.Equal("2022-10-13 00:00:00", createDataRange!["start"]);

            Assert.Equal("2022-10-14 00:00:00", createDataRange!["end"]);

            Assert.Equal("百度一下", obj.Data!["hyperLinkContext"]);

            Assert.Equal("https://www.baidu.com", obj.Data!["hyperLinkUrl"]);

            var departments = obj.Data!["departments"] as List<IDictionary<string, object>>;

            Assert.Equal("111", departments![0]["id"]);

            Assert.Equal("研发部", departments![0]["name"]);

            var leaders = obj.Data["leaders"] as List<object>;

            Assert.Equal("azirliang", leaders![0]);

            Assert.Equal("azir", leaders![1]);

            var role = obj.Data["role"] as IDictionary<string, object>;

            Assert.Equal("2222", role!["id"]);

            Assert.Equal("role2222", role!["name"]);

            Assert.Equal("10086", obj.Data!["phone1"]);

            Assert.Equal("10010", obj.Data!["phone2"]);

            var suggests = obj.Data["suggests"] as List<object>;

            Assert.Equal("aaa", suggests![0]);

            Assert.Equal("bbb", suggests![1]);

            Assert.Equal("aaa", obj.Data["suggest1"]);

            Assert.Equal("bbb", obj.Data["suggest2"]);
        }
    }
}