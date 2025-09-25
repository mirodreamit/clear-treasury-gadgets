delete from IdentityServer.AnonymousUser
delete from IdentityServer.UserCredential
delete from IdentityServer.UserDetail
delete from IdentityServer.[User]

delete from GD.GadgetCategory
delete from GD.Gadget
delete from GD.Category
delete from GD.[User]

insert into IdentityServer.[User] (id, Identifier, IsBlocked, IsSuperAdmin, CreatedAt, UpdatedAt)
select
*
from
	(
		select id = cast('3d33e362-e81a-4ca4-91c2-9d10ce8db0aa' as uniqueidentifier), Identifier = 'auth|12345', IsBlocked = 0, IsSuperAdmin = 0
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from IdentityServer.[User])

insert into IdentityServer.[UserCredential] (id, PasswordHash, CreatedAt, UpdatedAt)
select
*
from
	(
		select id = cast('3d33e362-e81a-4ca4-91c2-9d10ce8db0aa' as uniqueidentifier), PasswordHash = '100000.JKcE38Tri3XCJXOzxXtw1A==.pUv/YBPNsK6RQ5iB5wYWiSY9HzbUDbdmNYzJGC99/4o='
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from IdentityServer.[UserCredential])

insert into IdentityServer.[UserDetail] (id, Email, DisplayName, CreatedAt, UpdatedAt)
select
*
from
	(
		select id = cast('3d33e362-e81a-4ca4-91c2-9d10ce8db0aa' as uniqueidentifier), Email = 'test123@ct.uk', DisplayName = 'Seed'
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from IdentityServer.[UserDetail])

insert into GD.[User] (id, identifier, createdat, updatedat)
select
*
from
	(
		select id = cast('11311362-e81a-4ca4-91c2-9d10ce8d4122' as uniqueidentifier), Identifier = 'auth|12345'
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from IdentityServer.[UserDetail])

insert into GD.Category (id, Name, LastModifiedByUserId, createdat, updatedat)
select
*
from 
	(
		select id = cast('5e13e362-e81a-4ca4-91c2-9d10ce8db0aa' as uniqueidentifier), Name = 'Mobile', LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
		union all 
		select id = cast('2c18393a-17c9-4e41-a5b0-f9ad9d7408b6' as uniqueidentifier), Name = 'WiFi', LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
		union all 
		select id = cast('4d1763be-7e70-42b3-9e95-ca83a7b62e91' as uniqueidentifier), Name = 'TV', LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from GD.Category)

insert into GD.Gadget (id, Name, Description, StockQuantity, LastModifiedByUserId, createdat, updatedat)
select
*
from 
	(
		select id = cast('118feb77-3e7b-4f6f-ab47-1541d2355e27' as uniqueidentifier), Name = 'Flat screen TV', Description = 'It is a super machine', StockQuantity = 100, LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
		union all 
		select id = cast('dca26d57-1067-42ca-a9ac-a67dc820b8b3' as uniqueidentifier), Name = 'Coolest mobile phone', Description = null, StockQuantity = 200, LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from GD.Gadget)

insert into GD.GadgetCategory(id, GadgetId, CategoryId, ordinal, LastModifiedByUserId, createdat, updatedat)
select
*
from 
	(
		select Id = cast('599ee8b6-fdb7-4132-8eaa-e7d29bfc4712' as uniqueidentifier), GadgetId = cast('118feb77-3e7b-4f6f-ab47-1541d2355e27' as uniqueidentifier), CategoryId = cast('5e13e362-e81a-4ca4-91c2-9d10ce8db0aa' as uniqueidentifier), Ordinal = 1, LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
		union all 
		select Id = cast('dac2f638-b723-4d08-9977-931ccd515ea8' as uniqueidentifier), GadgetId = cast('118feb77-3e7b-4f6f-ab47-1541d2355e27' as uniqueidentifier), CategoryId = cast('2c18393a-17c9-4e41-a5b0-f9ad9d7408b6' as uniqueidentifier), Ordinal = 2, LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'

		union all 

		select Id = cast('ee16b5c9-beb9-40b8-84ae-41d7a4a7291f' as uniqueidentifier), GadgetId = cast('dca26d57-1067-42ca-a9ac-a67dc820b8b3' as uniqueidentifier), CategoryId = cast('5e13e362-e81a-4ca4-91c2-9d10ce8db0aa' as uniqueidentifier), Ordinal = 2, LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
		union all 
		select Id = cast('7f884861-4ba7-4820-8426-d72c7c96b3dc' as uniqueidentifier), GadgetId = cast('dca26d57-1067-42ca-a9ac-a67dc820b8b3' as uniqueidentifier), CategoryId = cast('4d1763be-7e70-42b3-9e95-ca83a7b62e91' as uniqueidentifier), Ordinal = 1, LastModifiedByUserId = '11311362-e81a-4ca4-91c2-9d10ce8d4122'
	) t
	cross apply
	(
		select CreatedAt = getdate(), UpdatedAt = getdate()
	) stamp
where
	t.id not in (select id from GD.GadgetCategory)





