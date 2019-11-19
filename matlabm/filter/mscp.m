%多元散射校正程序（Multiplicative Scatter Correction，MSC），用于处理校正集外光谱，处理校正集光谱的程序见msc。
% input 
% x:吸光度矩阵；
% mx:校正集的平均光谱；
% output
% xc:处理后的吸光度矩阵；

% reference : Applied Spectroscopy, Volumn 39,Number 4, 1985, 641-646
function[xc]=mscp(x,mx);

	x=x';
	[m,n]=size(x);

	mx=mx';
	x2=mean(x');
	x3=mean(mx);
	b(m)=zeros;
	a(m)=zeros;
	xc(m,n)=zeros;
		for i=1:m;
			b(i)=(sum((x(i,:)-x2(i)).*(mx-x3)))/sum((mx-x3).*(mx-x3));
			a(i)=x2(i)-b(i)*x3;
			xc(i,:)=(x(i,:)-a(i))/b(i);
		end
	xc=xc';
end






 