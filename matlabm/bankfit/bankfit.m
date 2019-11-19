function [ratio,ad]=bankfit(x,y);
% 库光谱拟合交互验证
% 输入：x，库样品光谱；y，待拟合样品光谱；
% 输出：ad，参与拟合的库光谱样品序号，ratio，参与拟合的库光谱的相应比例，
	[ra] = spectrafit(x,y);
	ra=ra';
	[B,INDEX]=sort(ra);
	ad1=find(B>0); 
	[m1,n1]=size(ad1);
	[m2,n2]=size(B);
	for i=1:n1
		ratio(i)=B(n2-i+1);
		ad(i)=INDEX(n2-i+1);
	end
	
	ratio=ratio'/sum(ratio);
	ad = ad';
end
